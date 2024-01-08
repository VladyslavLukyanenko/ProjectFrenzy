using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.Product
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Product
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("shopify_product_id")]
        public long ShopifyProductId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("variants")]
        public List<Variant> Variants { get; set; }

        [JsonProperty("options")]
        public List<Option> Options { get; set; }

        [JsonProperty("images")]
        public List<Image> Images { get; set; }
        
        [JsonIgnore]
        public string DefaultPicture => Images.Select(_ => _.Src).FirstOrDefault();
    }
}
