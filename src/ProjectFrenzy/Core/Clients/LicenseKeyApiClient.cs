using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Clients
{
  public class LicenseKeyApiClient : ApiClientBase, ILicenseKeyApiClient
  {
    private const string ProductId = "5efe8873e44d9d6f87d69a33";

    public LicenseKeyApiClient(ProjectIndustriesApiConfig apiConfig) : base(apiConfig)
    {
    }

    public async Task<AuthenticationResult> Authenticate(string licenseKey, string hwid, CancellationToken ct = default)
    {
      string path = $"/licenseKeys/{ProductId}/authenticate?sessionId={WebUtility.UrlEncode(hwid)}";
      try
      {
        var message = new HttpRequestMessage(HttpMethod.Get, ApiConfig.ResolveUrl(path))
        {
          Headers = {{"Auth", licenseKey}}
        };

        var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
        string json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<AuthenticationResult>(json);
      }
      catch (Exception)
      {
        return AuthenticationResult.CreateUnkownError();
      }
    }
  }
}