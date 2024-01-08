using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.FlashSale
{
  [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
  public class ProductDetail
  {
    [JsonProperty("price_range")] public PriceRange PriceRange { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("image_urls")] public string[] ImageUrls { get; set; }
        
    [JsonIgnore]
    public string DefaultPicture => ImageUrls.FirstOrDefault();
  }
}