using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.Discord
{
    public class Footer
    {
        [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
    }
}
