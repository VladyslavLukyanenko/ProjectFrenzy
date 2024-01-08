using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using DynamicData;
using DynamicData.Binding;
using ProjectFrenzy.AvaloniaUI.Infra.Converters;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Model.FlashSale;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ViewModels;
using Splat;

namespace ProjectFrenzy.AvaloniaUI.Infra.Services
{
  public class PreloadService : IPreloadService
  {
    private readonly IFlashsaleService _flashsaleService;
    private readonly IFlashsalesApiClient _flashsalesApiClient;

    public PreloadService(IFlashsaleService flashsaleService, IFlashsalesApiClient flashsalesApiClient)
    {
      _flashsaleService = flashsaleService;
      _flashsalesApiClient = flashsalesApiClient;
    }

    public async Task PreAuthPreloadAsync(CancellationToken ct = default)
    {
      await PreloadDrops(ct);
    }

    public async Task PostAuthPreloadAsync(CancellationToken ct = default)
    {
      await Task.WhenAll(
        Resolve<IDiscordSettingsService>().RefreshAsync(ct),
        Resolve<IProfilesService>().InitializeAsync(ct),
        Resolve<ICheckoutService>().RefreshAsync(ct),
        Resolve<IStatsService>().RefreshAsync(ct),
        Resolve<IProxiesService>().InitializeAsync(ct),
        Resolve<IEmailService>().InitializeAsync(ct),
        Resolve<ICountriesService>().InitializeAsync(ct)
      );

      // they're lazy singletons, so let's preload them
      Resolve<DashboardViewModel>();
      Resolve<ProfilesGridViewModel>();
      Resolve<TasksGridViewModel>();
      Resolve<ProxiesViewModel>();
      Resolve<SettingsViewModel>();
    }

    private static T Resolve<T>()
    {
      return Locator.Current.GetService<T>();
    }

    private async Task PreloadDrops(CancellationToken ct = default)
    {
      var tcs = new TaskCompletionSource<bool>();

      var disposable = new CompositeDisposable();
      _flashsaleService.Flashsales.Connect()
        .Filter(_ => _.IsAvailable())
        .Bind(out var drops)
        .DisposeMany()
        .Subscribe()
        .DisposeWith(disposable);

      drops.ToObservableChangeSet()
        .Subscribe(c =>
        {
          Task.Run(async () =>
          {
            Console.WriteLine("Preloading images...");
            var picturesToPreload = drops.SelectMany(d => new[] {d.ImageUrls.Select(_ => _.ToString().FirstOrDefault())}
                .Union(d.ProductDetails.SelectMany(r => r.ImageUrls)))
              .ToArray();

            var imageConvTasks = picturesToPreload.Select(picture =>
              Task.Factory.StartNew(() =>
              {
#if DEBUG
                return null;
#endif
                try
                {
                  return BitmapValueConverter.Instance.Convert(picture, typeof(IBitmap), null, null);
                }
                catch (Exception e)
                {
                  Console.WriteLine(e);
                  return null;
                }
              }, ct, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            var images = await Task.WhenAll(imageConvTasks);
            Console.WriteLine("Preloaded images count: {0}", images.Length);

            disposable.Dispose();
            tcs.SetResult(true);
          }, ct);
        })
        .DisposeWith(disposable);


      Console.WriteLine("Loading flashsales...");
      var flashsales = await _flashsalesApiClient.GetAllAsync(ct);
      Console.WriteLine("Loaded {0} flashsales", flashsales.Count);

      if (flashsales.Count == 0)
      {
        disposable.Dispose();
        tcs.SetResult(true);
      }
      else
      {
        _flashsaleService.AddOrUpdate(flashsales);
      }

      await tcs.Task;
    }
  }
}