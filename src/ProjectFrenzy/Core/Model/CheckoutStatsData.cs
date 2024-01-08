using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model
{
  public class CheckoutStatsData
  {
    [JsonProperty("price")]
    public decimal Price { get; set; }
  }
}