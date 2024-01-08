using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.FlashSale
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class PriceRange
    {
        [JsonProperty("min")]
        public double Min { get; set; }

        [JsonProperty("max")]
        public double Max { get; set; }
    }

}
