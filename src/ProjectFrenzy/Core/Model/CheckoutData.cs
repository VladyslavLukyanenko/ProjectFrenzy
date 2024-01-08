using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model
{
  public class CheckoutData
  {
    [JsonProperty("image")] public string Image { get; set; }
    [JsonProperty("success")] public bool IsSuccess { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("price")] public decimal Price { get; set; }
  }
}