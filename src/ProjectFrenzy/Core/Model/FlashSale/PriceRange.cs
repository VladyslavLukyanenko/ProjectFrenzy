using System;
using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.FlashSale
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class PriceRange
    {
        [JsonProperty("min")]
        public double Min { get; set; }

        [JsonProperty("max")]
        public double Max { get; set; }

        public string ToString(string currency)
        {
            if (Math.Abs(Min - Max) < .0001D)
            {
                return $"{Min:N2} {currency}";
            }

            return $"{Min:N2} - {Max:N2} {currency}";
        }
    }

}
