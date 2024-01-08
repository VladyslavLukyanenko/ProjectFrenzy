using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.Discord
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Image
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
