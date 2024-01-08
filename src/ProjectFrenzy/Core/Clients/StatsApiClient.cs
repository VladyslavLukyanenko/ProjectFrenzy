using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
  public class StatsApiClient : ApiClientBase, IStatsApiClient
  {
    public StatsApiClient(ProjectIndustriesApiConfig apiConfig) : base(apiConfig)
    {
    }
    public async Task<bool> SubmitStatsAsync(CheckoutStats stats, string licenseKey, CancellationToken ct = default)
    {
      var message = new HttpRequestMessage(HttpMethod.Post, ApiConfig.ResolveUrl("/statistics"))
      {
        Headers = {{"Auth", licenseKey}},
        Content = new StringContent(JsonConvert.SerializeObject(stats), Encoding.UTF8, "application/json")
      };

      var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
      return response.IsSuccessStatusCode;
    }

  }
}