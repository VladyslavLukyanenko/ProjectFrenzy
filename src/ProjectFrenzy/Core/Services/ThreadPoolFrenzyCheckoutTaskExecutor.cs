using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Events;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Model.AndroidPayload;
using ProjectFrenzy.Core.Model.AndroidResponse;
using ProjectFrenzy.Core.Model.CheckoutPayload;
using ProjectFrenzy.Core.Model.FlashSale;
using ProjectFrenzy.Core.Model.Product;
using ReactiveUI;

namespace ProjectFrenzy.Core.Services
{
  public class ThreadPoolFrenzyCheckoutTaskExecutor : IFrenzyCheckoutTaskExecutor
  {
    private readonly IEmulatorClient _emulatorClient;
    private readonly IFrenzyCheckoutApiClient _frenzyCheckoutApiClient;
    private readonly IProductsApiClient _productsApiClient;
    private readonly IMessageBus _messageBus;
    private readonly IProxiesService _proxiesService;

    private readonly Dictionary<FrenzyCheckoutTask, CancellationTokenSource> _taskCancellationsDict =
      new Dictionary<FrenzyCheckoutTask, CancellationTokenSource>();

    private static readonly Dictionary<string, IList<Product>> ProductsCache = new Dictionary<string, IList<Product>>();
    // private static readonly SemaphoreSlim ProductsSemaphore = new SemaphoreSlim(1, 1);

    public ThreadPoolFrenzyCheckoutTaskExecutor(IEmulatorClient emulatorClient,
      IFrenzyCheckoutApiClient frenzyCheckoutApiClient, IProductsApiClient productsApiClient, IMessageBus messageBus,
      IProxiesService proxiesService)
    {
      _emulatorClient = emulatorClient;
      _frenzyCheckoutApiClient = frenzyCheckoutApiClient;
      _productsApiClient = productsApiClient;
      _messageBus = messageBus;
      _proxiesService = proxiesService;
    }


    public async Task ExecuteAsync(FrenzyCheckoutTask task, CancellationToken ct = default)
    {
      if (task.Status.IsRunning)
      {
        return;
      }

      var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
      _taskCancellationsDict[task] = cts;
      await Task.Factory.StartNew(() =>
      {
        try
        {
          ProcessAsync(task, cts.Token);
        }
        catch (OperationCanceledException)
        {
          task.Status = CheckoutStatus.Cancelled;
        }
        catch (Exception exc)
        {
          // todo: log it somewhere
          task.Status = CheckoutStatus.UnknownError("Unhandled: " + exc.Message);
        }
        finally
        {
          _taskCancellationsDict.Remove(task);
        }
      }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Cancel(FrenzyCheckoutTask task)
    {
      if (_taskCancellationsDict.TryGetValue(task, out var cts))
      {
        cts.Cancel(true);
      }
    }

    public void CancelAllTasks()
    {
      foreach (var cts in _taskCancellationsDict.Values.ToArray())
      {
        cts.Cancel(true);
      }
    }

    public async Task ExecuteAsync(IEnumerable<FrenzyCheckoutTask> tasks, CancellationToken ct = default)
    {
      var hotTasks = tasks.Select(t => ExecuteAsync(t, ct));
      await Task.WhenAll(hotTasks);
    }

    private void ProcessAsync(FrenzyCheckoutTask task, CancellationToken ct = default)
    {
      if (task.UseProxies && !_proxiesService.HasAnyProxy)
      {
        task.Status = CheckoutStatus.NoProxiesConfigured;
        return;
      }

      var response = GetEmulatorAsync(task, ct);
      if (response == null)
      {
        task.Status = CheckoutStatus.NoEmulatorAvailable;
        return;
      }

      if (task.AssignedEmail == null)
      {
        task.Status = CheckoutStatus.NotEmailAvailable;

        return;
      }

//
// #if DEBUG
//       task.Flashsale.StartedAt = DateTime.UtcNow.AddMinutes(1);
// #endif
      ct.ThrowIfCancellationRequested();
      CheckoutPayload checkoutPayload =
        ConstructCheckoutPayload(task.SelectedProfile, task.Flashsale, response, task.AssignedEmail.Value);
      var rootPayload = new CheckoutPayloadRoot
      {
        Checkout = checkoutPayload
      };

      AwaitForUpcomingAsync(task, ct);
      var product = GetProductAsync(task, ct);

      ct.ThrowIfCancellationRequested();
      var variant = ChooseVariant(task, product);
      if (!variant.HasValue)
      {
        task.Status = CheckoutStatus.VariantNotFound;
        return;
      }

      var variantId = variant.Value.variantId;
      checkoutPayload.SelectVariant(variantId);

      var timer = Stopwatch.StartNew();
      DelayUtilCheckoutTimeAsync(task, ct);

      task.Status = CheckoutStatus.Submitting;
      HttpResponseMessage rawCheckoutResponse =
        _frenzyCheckoutApiClient.CheckoutAsync(rootPayload, task.UseProxies, ct).GetAwaiter().GetResult();
      timer.Stop();


      task.Status = CheckoutStatus.ProcessingResults;
      // NOTE: CancellationToken.None passed into next calls because we actually made checkout and next steps just to get info about results
      Guid checkoutId = _frenzyCheckoutApiClient.ExtractCheckoutIdAsync(rawCheckoutResponse).GetAwaiter().GetResult();
      CheckoutResult checkoutResult = _frenzyCheckoutApiClient
        .ProcessCheckoutAsync(checkoutId, task.UseProxies, CancellationToken.None)
        .GetAwaiter()
        .GetResult();
// #if DEBUG
//       var random = new Random((int) DateTime.Now.Ticks);
//       checkoutResult.TotalPrice = (decimal) (random.NextDouble() * random.Next(100) + 100);
//       if ((int) checkoutResult.TotalPrice % 2 == 0)
//       {
//         checkoutResult.Status = CheckoutStatus.OutOfStock;
//       }
// #endif

      task.Status = checkoutResult.Status;
      task.CheckoutDuration = timer.Elapsed;
      _messageBus.SendMessage(new FrenzyCheckoutTaskCompleted(task, checkoutResult, checkoutPayload));
    }

    private AndroidResponse GetEmulatorAsync(FrenzyCheckoutTask task, CancellationToken ct)
    {
      task.Status = CheckoutStatus.AwaitingForFreeEmulator;
      var payload = AndroidPayload.FromFlashsale(task.Flashsale);
      AndroidResponse response;
      var callbacks = new TokenRequestLifetimeCallbacks
      {
        EmulatorSelected = () => task.Status = CheckoutStatus.EmulatorSelected,
        RetryAuthorizationFailed = attempt => task.Status = CheckoutStatus.RetryAuthorization(attempt)
      };

      response = _emulatorClient.GetToken(payload, callbacks, task.PreferredEmulator, ct);
      return response;
    }

    private Product GetProductAsync(FrenzyCheckoutTask task, CancellationToken ct)
    {
      task.Status = CheckoutStatus.FetchingProduct;
      IList<Product> products;
      lock (task.Flashsale)
      {
        var cacheKey = $"{task.Flashsale.Id}_{task.Flashsale.Password}_{task.Flashsale.Title}";
        if (!ProductsCache.TryGetValue(cacheKey, out products))
        {
          products = _productsApiClient.GetProductByPasswordAsync(task.Flashsale.Password, ct).GetAwaiter().GetResult();
          ProductsCache[cacheKey] = products;
        }
      }

      var idx = task.Flashsale.ProductDetails.IndexOf(task.Product);
      var product = products[idx];
      return product;
    }


    private void DelayUtilCheckoutTimeAsync(FrenzyCheckoutTask task, CancellationToken ct)
    {
      var checkoutDelay = task.Flashsale.GetDelay();
      if (checkoutDelay > TimeSpan.Zero)
      {
        var d = Observable.Interval(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
          .Subscribe(o => task.Status = CheckoutStatus.DelayUntilCheckoutTime(task.Flashsale.GetDelay()));

        var r = ct.Register(() => { d.Dispose(); });
        Task.Delay(checkoutDelay, ct).GetAwaiter().GetResult();

        r.Unregister();
        d.Dispose();
      }

      Task.Delay(TimeSpan.FromMilliseconds(task.CheckoutDelay), ct).GetAwaiter().GetResult();
    }

    private (long variantId, string selectedOption)? ChooseVariant(FrenzyCheckoutTask task, Product product)
    {
      task.Status = CheckoutStatus.SelectingProductVariant;
      return task.Mode switch
      {
        CheckoutMode.Random => SelectRandomly(product),
        CheckoutMode.OnlySize => SelectStrictBySize(task, product),
        CheckoutMode.Preference => SelectStrictBySize(task, product) ?? SelectRandomly(product),
        _ => null
      };
    }

    (long variantId, string selectedOption)? SelectStrictBySize(FrenzyCheckoutTask task, Product product)
    {
      foreach (var size in task.SelectedSizes)
      {
        var normalizedSize = size.ToLower();
        var result = product
          .Variants
          .Where(_ => _.InventoryQuantity > 0)
          .Select(variant => (ValueTuple<long, string>?) (variant.ShopifyVariantId, variant.Title))
          .FirstOrDefault(variant => variant.Value.Item2.ToLower().Contains(normalizedSize));

        if (result != null)
        {
          return result;
        }
      }

      return null;
    }

    (long variantId, string selectedOption)? SelectRandomly(Product product)
    {
      int variantCount = product.Variants.Count;
      var random = new Random((int) DateTime.Now.Ticks);

      for (int retries = 0; retries < 50; retries++)
      {
        int index = random.Next(variantCount - 1);
        var variant = product.Variants[index];
        if (variant.InventoryQuantity > 0)
        {
          return (variant.ShopifyVariantId, variant.Title);
        }
      }

      return null;
    }

    private static CheckoutPayload ConstructCheckoutPayload(Profile profile, Flashsale flashsale,
      AndroidResponse androidResponse, string email)
    {
      Dropzone spoofer = null;
      if (flashsale.Dropzone.Count > 0)
      {
        spoofer = new Spoofer(flashsale.Dropzone).Spoof();
      }

      var payload = new CheckoutPayload
      {
        AccelerometerData = new Data {X = 1, Y = 1, Z = 1},
        Email = email,
        FlashsalePassword = flashsale.Password,
        ShopId = flashsale.Shop.Id,
        GyroData = new Data {X = 1, Y = 1, Z = 1},
        LastDigits = androidResponse.Card,
        LineItems = new List<LineItem>(new[] {new LineItem {Quantity = 1}}),
        Location = new Location
        {
          Altitude = "0.0",
          Course = "0.0",
          Lat = flashsale.Dropzone.Count == 0 ? 0 : spoofer.Lat,
          Lng = flashsale.Dropzone.Count == 0 ? 0 : spoofer.Lng,
          Speed = "0.0"
        },
        Payment = new Payment
        {
          Source = new Source
          {
            PaymentToken = new PaymentToken
            {
              PaymentData = androidResponse.Token,
              Type = "google_pay"
            }
          }
        },
        ShippingAddress = MapToAddress(profile.ShippingAddress),
        BillingAddress =
          MapToAddress(profile.IsShippingSameAsBilling ? profile.ShippingAddress : profile.BillingAddress)
      };
      if (string.IsNullOrWhiteSpace(payload.BillingAddress.City))
      {
        payload.BillingAddress.City = "ABCD";
      }

      if (string.IsNullOrWhiteSpace(payload.ShippingAddress.City))
      {
        payload.ShippingAddress.City = "ABCD";
      }

      if (string.IsNullOrWhiteSpace(payload.BillingAddress.ProvinceCode))
      {
        payload.BillingAddress.ProvinceCode = "AA";
      }

      if (string.IsNullOrWhiteSpace(payload.ShippingAddress.ProvinceCode))
      {
        payload.ShippingAddress.ProvinceCode = "AA";
      }

      return payload;
    }

    private static Address MapToAddress(CheckoutAddress billingAddress)
    {
      return new Address
      {
        Address1 = billingAddress.AddressLine1.OrEmpty(),
        Address2 = billingAddress.AddressLine2.OrEmpty(),
        City = billingAddress.City.OrEmpty(),
        Phone = billingAddress.PhoneNumber.OrEmpty(),
        Zip = billingAddress.ZipCode.OrEmpty(),
        CountryCode = billingAddress.CountryId.OrEmpty(),
        FirstName = billingAddress.FirstName.OrEmpty(),
        LastName = billingAddress.LastName.OrEmpty(),
        ProvinceCode = billingAddress.ProvinceCode.OrEmpty()
      };
    }

    private void AwaitForUpcomingAsync(FrenzyCheckoutTask task, CancellationToken ct)
    {
      if (task.Flashsale.IsUpcoming())
      {
        var delay = CalculateDelay();
        if (delay > TimeSpan.Zero)
        {
          var d = Observable.Interval(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(o => task.Status = CheckoutStatus.AwaitingForUpcoming(CalculateDelay()));
          var r = ct.Register(() => { d.Dispose(); });

          Task.Delay(CalculateDelay(), ct).GetAwaiter().GetResult();


          r.Unregister();
          d.Dispose();
        }
      }

      TimeSpan CalculateDelay() => task.Flashsale.GetDelay() - TimeSpan.FromMilliseconds(3_000);
    }
  }

  internal static class StrExts
  {
    public static string OrEmpty(this string str) => string.IsNullOrEmpty(str) ? string.Empty : str;
  }
}