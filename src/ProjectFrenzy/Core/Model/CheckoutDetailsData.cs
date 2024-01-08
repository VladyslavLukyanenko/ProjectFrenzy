using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model
{
    public class CheckoutDetailsData : CheckoutData
    {
        [JsonProperty("_id")] public string Id { get; set; }
        [JsonProperty("date")] public int Date { get; set; }
    }
}