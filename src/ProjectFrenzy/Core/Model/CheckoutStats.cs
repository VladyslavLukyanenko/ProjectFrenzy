using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model
{
  public class CheckoutStats
  {
    [JsonProperty("data")]
    public CheckoutStatsData Data { get; set; } = new CheckoutStatsData();

    public static CheckoutStats Create(CheckoutResult result) => new CheckoutStats {Data = {Price = result.TotalPrice}};
  }
}