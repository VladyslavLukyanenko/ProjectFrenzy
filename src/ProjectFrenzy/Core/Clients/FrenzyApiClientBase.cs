using System.Net.Http;

namespace ProjectFrenzy.Core.Clients
{
    public abstract class FrenzyApiClientBase
    {
        protected readonly HttpClient HttpClient = new HttpClient();

        protected FrenzyApiClientBase()
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Frenzy/1 CFNetwork/978.0.7 Darwin/18.7.0");
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }
}