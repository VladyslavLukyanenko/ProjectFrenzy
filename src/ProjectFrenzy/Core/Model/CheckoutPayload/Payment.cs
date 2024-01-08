using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.CheckoutPayload
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Payment
    {
        [JsonProperty("source")]
        public Source Source { get; set; }
    }
}
