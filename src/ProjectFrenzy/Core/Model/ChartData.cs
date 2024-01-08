using System;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model
{
    public class ChartData
    {
        [JsonProperty("totalSpent")] public decimal TotalSpent { get; set; }

        [JsonProperty("maxSpent")] public decimal MaxSpent { get; set; }

        [JsonProperty("minSpent")] public decimal MinSpent { get; set; }

        [JsonProperty("entries")] public ChartEntryData[] Entries { get; set; } = Array.Empty<ChartEntryData>();

        public override string ToString()
        {
            return
                $"{nameof(TotalSpent)}={TotalSpent:C} {nameof(TotalSpent)}={MaxSpent:C} {nameof(TotalSpent)}={MinSpent:C} {nameof(Entries)}={Entries.Length}";
        }
    }
}