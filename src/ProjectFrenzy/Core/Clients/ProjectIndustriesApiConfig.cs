using System;

namespace ProjectFrenzy.Core.Clients
{
    public class ProjectIndustriesApiConfig
    {
        public string ApiHostName { get; set; }

        public Uri ResolveUrl(string relative)
        {
            if (!relative.StartsWith("/"))
            {
                relative = "/" + relative;
            }

            return new Uri(ApiHostName + relative);
        }
    }
}