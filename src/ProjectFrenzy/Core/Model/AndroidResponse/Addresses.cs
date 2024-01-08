using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.AndroidResponse
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Addresses
    {
        [JsonProperty("billing")]
        public Address Billing { get; set; }

        [JsonProperty("shipping")]
        public Address Shipping { get; set; }
    }
}
