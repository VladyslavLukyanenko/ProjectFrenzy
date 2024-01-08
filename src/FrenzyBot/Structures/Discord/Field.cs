using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.Discord
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Field
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("inline")]
        public bool Inline { get; set; }
    }
}
