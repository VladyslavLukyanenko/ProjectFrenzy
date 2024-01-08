using ProjectFrenzy.Core.Clients;

namespace ProjectFrenzy.Core
{
    public class ApplicationConfig
    {
        public SecurityConfig Security { get; set; } = new SecurityConfig();
        public ProjectIndustriesApiConfig ProjectIndustriesApi { get; set; } = new ProjectIndustriesApiConfig();
    }
}