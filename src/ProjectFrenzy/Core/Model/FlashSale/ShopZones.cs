using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.FlashSale
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public partial class ShopZones
    {
        [JsonProperty("google_pay_merchant_id")]
        public long GooglePayMerchantId { get; set; }
    }
}
