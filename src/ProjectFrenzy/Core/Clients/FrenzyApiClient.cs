using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
    public class FrenzyApiClient : ApiClientBase, IFrenzyApiClient
    {
        public FrenzyApiClient(ProjectIndustriesApiConfig apiConfig) : base(apiConfig)
        {
        }

        public async Task<string> DecodeAsync(string licenseKey, string content, CancellationToken ct = default)
        {
            try
            {
                var encodedContent = WebUtility.UrlEncode(content);
                var message =
                    new HttpRequestMessage(HttpMethod.Get, ApiConfig.ResolveUrl("/frenzy/api/" + encodedContent))
                    {
                        Headers = {{"Auth", licenseKey}}
                    };

                var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<FrenzyStatisticsResponseModel> GetStatsAsync(string licenseKey,
            CancellationToken ct = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, ApiConfig.ResolveUrl("/frenzy/api/stats"))
            {
                Headers = {{"Auth", licenseKey}}
            };

            var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FrenzyStatisticsResponseModel>(json);
        }
    }
}