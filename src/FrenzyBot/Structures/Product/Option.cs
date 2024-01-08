using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace FrenzyBot.Structures.Product
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Option
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }
    }
}
