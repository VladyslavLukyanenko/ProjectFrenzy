using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Clients
{
  public class PeriodicDataContainer<T> where T : new()
  {
    [JsonProperty("daily")] public T Daily { get; set; } = new T();

    [JsonProperty("monthly")] public T Monthly { get; set; } = new T();

    [JsonProperty("yearly")] public T Yearly { get; set; } = new T();
  }
}