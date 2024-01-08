using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using ProjectFrenzy.Core.Model.FlashSale;

namespace ProjectFrenzy.Core.Model.AndroidPayload
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

        public static AndroidPayload FromFlashsale(Flashsale flashsale) => new AndroidPayload
        {
            GatewayMerchantId = flashsale.Shop.ShopZones.GooglePayMerchantId.ToString(),
            TotalPrice = flashsale.PriceRange.Max.ToString(CultureInfo.InvariantCulture),
            Currency = flashsale.Shop.Currency
        };  
    }
}
