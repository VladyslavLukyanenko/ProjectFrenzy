using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.FlashSale
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class ProductDetail
    {
        [JsonProperty("price_range")]
        public PriceRange PriceRange { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
