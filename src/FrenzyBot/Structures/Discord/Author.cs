using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.Discord
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Author
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
    }
}
