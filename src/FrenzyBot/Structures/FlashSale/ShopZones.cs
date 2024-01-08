using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.FlashSale
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public partial class ShopZones
    {
        [JsonProperty("google_pay_merchant_id")]
        public long GooglePayMerchantId { get; set; }
    }
}
