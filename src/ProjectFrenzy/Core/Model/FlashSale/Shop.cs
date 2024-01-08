using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.FlashSale
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Shop
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("shop_zones")]
        public ShopZones ShopZones { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
