using System;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model
{
    public class ChartEntryData
    {
        private DateTime? _spendingDate;
        [JsonProperty("timeSpan")]
        public long TimeSpan { get; set; }

        [JsonProperty("spending")]
        public decimal Spending { get; set; }

        [JsonIgnore]
        public DateTime SpendingDate =>
            _spendingDate ?? (_spendingDate = DateTimeOffset.FromUnixTimeSeconds(TimeSpan).DateTime).Value;

        public override string ToString()
        {
            return $"{Spending:C} {DateTimeOffset.FromUnixTimeSeconds(TimeSpan):s}";
        }
    }
}