using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.AndroidResponse
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
