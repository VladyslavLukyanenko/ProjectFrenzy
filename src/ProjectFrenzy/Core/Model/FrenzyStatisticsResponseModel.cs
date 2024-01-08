using Newtonsoft.Json;
using ProjectFrenzy.Core.Clients;

namespace ProjectFrenzy.Core.Model
{
  public class FrenzyStatisticsResponseModel
  {
    [JsonProperty("successfulCheckouts")]
    public PeriodicDataContainer<int> SuccessfulCheckouts { get; set; } = new PeriodicDataContainer<int>();

    [JsonProperty("failedCheckouts")]
    public PeriodicDataContainer<int> FailedCheckouts { get; set; } = new PeriodicDataContainer<int>();

    [JsonProperty("chart")]
    public PeriodicDataContainer<ChartData> Chart { get; set; } = new PeriodicDataContainer<ChartData>();
  }
}