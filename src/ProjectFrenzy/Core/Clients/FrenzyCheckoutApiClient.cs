using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Model.CheckoutPayload;
using ProjectFrenzy.Core.Services;

namespace ProjectFrenzy.Core.Clients
{
  public class FrenzyCheckoutApiClient : IFrenzyCheckoutApiClient
  {
    private readonly IProxiesService _proxiesService;

    public FrenzyCheckoutApiClient(IProxiesService proxiesService)
    {
      _proxiesService = proxiesService;
    }

    private async Task<(HttpClient Client, Proxy Proxy)> CreateConfiguredHttpClient(bool useProxy)
    {
      var handler = new HttpClientHandler();
      Proxy proxy = null;
      handler.UseProxy = useProxy;
      if (useProxy)
      {
        while ((proxy = _proxiesService.GetUnusedProxy()) == null)
        {
          await Task.Delay(TimeSpan.FromMilliseconds(300));
        }

        handler.Proxy = proxy.ToWebProxy();
      }

      var client = new HttpClient(handler);
      client.DefaultRequestHeaders.Clear();
      client.DefaultRequestHeaders.Add("User-Agent", "okhttp/3.8.0");
      client.DefaultRequestHeaders.Add("Accept", "application/json");
      client.DefaultRequestHeaders.Add("Accept-Language", "en-US");

      return (client, proxy);
    }

    public async Task<HttpResponseMessage> CheckoutAsync(CheckoutPayloadRoot payloadRoot,
      bool useProxies, CancellationToken ct = default)
    {
      var body = new StringContent(JsonConvert.SerializeObject(payloadRoot), Encoding.UTF8, "application/json");
      while (true)
      {
        var client = await CreateConfiguredHttpClient(useProxies);
        try
        {
          var checkoutResponse =
            await client.Client.PostAsync("https://frenzy.shopifyapps.com/api/checkouts", body, ct);
          if (checkoutResponse.IsSuccessStatusCode)
          {
            return checkoutResponse;
          }
        }
        catch
        {
          // noop
        }
        finally
        {
          _proxiesService.TryReleaseProxy(client.Proxy);
        }
      }
    }

    public async Task<Guid> ExtractCheckoutIdAsync(HttpResponseMessage rawCheckoutResponse)
    {
      var checkoutResponseContent = await rawCheckoutResponse.Content.ReadAsStringAsync();
      return JObject.Parse(checkoutResponseContent)["checkout"]?["uuid"]?.ToObject<Guid>() ?? Guid.Empty;
    }

    public async Task<CheckoutResult> ProcessCheckoutAsync(Guid checkoutId, bool useProxies,
      CancellationToken ct = default)
    {
      while (true)
      {
        try
        {
          var details = await FetchCheckoutDetailsAsync(checkoutId, useProxies, ct);
          var pollContent = details.content;
          if (pollContent.IsQueued())
          {
            await Task.Delay(TimeSpan.FromMilliseconds(150), ct);
            continue;
          }

          var checkoutResult = new CheckoutResult
          {
            Status = pollContent.GetStatus(),
            TotalPrice = pollContent.TotalPrice.GetValueOrDefault(),
            StatusMessage = pollContent.GetNormalizedStatusMessage(),
            RawResponse = details.raw
          };
          
          return checkoutResult;
        }
        catch (TaskCanceledException)
        {
          if (ct.IsCancellationRequested)
          {
            throw;
          }
        }
        catch (OperationCanceledException)
        {
          if (ct.IsCancellationRequested)
          {
            throw;
          }
        }
        catch
        {
          // noop
        }
      }
    }

    private async Task<(CheckoutProcessingResponse content, string raw)> FetchCheckoutDetailsAsync(Guid checkoutId, bool useProxies,
      CancellationToken ct)
    {
      HttpResponseMessage pollResponse;
      (HttpClient Client, Proxy Proxy) client = default;
      try
      {
        client = await CreateConfiguredHttpClient(useProxies);
        pollResponse = await client.Client.GetAsync($"https://frenzy.shopifyapps.com/api/checkouts/{checkoutId}", ct);
      }
      finally
      {
        if (client.Proxy != null)
        {
          _proxiesService.TryReleaseProxy(client.Proxy);
        }
      }

      var raw = await pollResponse.Content.ReadAsStringAsync();
      var pollContent = JObject.Parse(raw)["checkout"]
        ?.ToObject<CheckoutProcessingResponse>();
      return (pollContent, raw);
    }

    private class CheckoutProcessingResponse
    {
      [JsonProperty("status")] public string Status { get; set; }
      [JsonProperty("normalized_errors")] public string NormalizedErrors { get; set; }
      [JsonProperty("total_price")] public decimal? TotalPrice { get; set; }

      public string GetNormalizedStatusMessage() => Status switch
      {
        "success" => "Success",
        "error" => NormalizedErrors,
        "out_of_stock" => "Out of Stock",
        _ => "Unknown error"
      };


      public CheckoutStatus GetStatus() => Status switch
      {
        "success" => CheckoutStatus.CheckoutSuccessful,
        "error" => CheckoutStatus.FailedCheckout(GetNormalizedStatusMessage()),
        "out_of_stock" => CheckoutStatus.OutOfStock,
        "payment_declined" => CheckoutStatus.PaymentDeclined,
        _ => CheckoutStatus.UnknownError(NormalizedErrors)
      };

      public bool IsQueued() => Status == "queued";
    }
  }
}