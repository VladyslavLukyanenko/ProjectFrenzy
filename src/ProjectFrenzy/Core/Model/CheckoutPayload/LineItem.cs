using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.CheckoutPayload
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class LineItem
    {
        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("variant_id")]
        public long VariantId { get; set; }
    }
}
