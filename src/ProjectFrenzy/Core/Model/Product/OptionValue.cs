using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.Product
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class OptionValue
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
