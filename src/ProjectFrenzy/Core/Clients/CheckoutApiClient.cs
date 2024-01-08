using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
  public class CheckoutApiClient : ApiClientBase, ICheckoutApiClient
  {
    public CheckoutApiClient(ProjectIndustriesApiConfig apiConfig) : base(apiConfig)
    {
    }

    public async Task<LastCheckoutsResponse> GetLastCheckoutsAsync(string licenseKey, CancellationToken ct = default)
    {
      var message = new HttpRequestMessage(HttpMethod.Get, ApiConfig.ResolveUrl("/checkouts/last/20"))
      {
        Headers = {{"Auth", licenseKey}}
      };

      var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
      string json = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<LastCheckoutsResponse>(json);
    }

    public async Task<bool> SubmitCheckoutAsync(CheckoutData data, string licenseKey, CancellationToken ct = default)
    {
      var message = new HttpRequestMessage(HttpMethod.Post, ApiConfig.ResolveUrl("/checkouts"))
      {
        Headers = {{"Auth", licenseKey}},
        Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
      };

      var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
      return response.IsSuccessStatusCode;
    }
  }
}