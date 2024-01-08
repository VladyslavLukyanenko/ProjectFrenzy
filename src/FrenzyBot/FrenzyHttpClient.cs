using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FrenzyBot
{
    class FrenzyHttpClient : HttpClient
    {
        private static HttpClient Client_ = new HttpClient();
        public FrenzyHttpClient()
        {
            Client_.DefaultRequestHeaders.Clear();
            Client_.DefaultRequestHeaders.Add("User-Agent", "Frenzy/1 CFNetwork/978.0.7 Darwin/18.7.0");
            Client_.DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }
}
