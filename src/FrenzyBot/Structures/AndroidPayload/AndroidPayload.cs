using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.User
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class AndroidPayload
    {
        [JsonProperty("gateway_merchant_id")]
        public string GatewayMerchantId { get; set; }
        [JsonProperty("total_price")]
        public string TotalPrice { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
