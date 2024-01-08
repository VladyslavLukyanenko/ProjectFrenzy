using Newtonsoft.Json;
using System;
using System.Reflection;

namespace FrenzyBot.Structures.Checkout
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class CheckoutResponse
    {
        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("normalized_errors")]
        public object NormalizedErrors { get; set; }

        [JsonProperty("total_price")]
        public object TotalPrice { get; set; }
    }
}
