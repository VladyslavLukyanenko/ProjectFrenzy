using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.User
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class CheckoutLogStructure
    {
        [JsonProperty("item")]
        public FlashSale.Flashsale Item { get; set; }

        [JsonProperty("title")]
        public string ProductTitle { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("checkoutSpeed")]
        public string CheckoutSpeed { get; set; }

        [JsonProperty("option")]
        public string Option { get; set; }

        [JsonProperty("delay")]
        public int CustomDelay { get; set; }

        [JsonProperty("shopname")]
        public string ShopName { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
