using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace FrenzyBot.Structures.Product
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Variant
    {
        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("shopify_variant_id")]
        public long ShopifyVariantId { get; set; }

        [JsonProperty("option_values")]
        public List<OptionValue> OptionValues { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }

        [JsonProperty("weight_unit")]
        public string WeightUnit { get; set; }

        [JsonProperty("sold_out")]
        public bool SoldOut { get; set; }

        [JsonProperty("requires_shipping")]
        public bool RequiresShipping { get; set; }

        [JsonProperty("images")]
        public List<object> Images { get; set; }

        [JsonProperty("inventory_quantity")]
        public long InventoryQuantity { get; set; }
    }
}
