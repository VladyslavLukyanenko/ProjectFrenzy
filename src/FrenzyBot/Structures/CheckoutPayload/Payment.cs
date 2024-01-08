using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.CheckoutPayload
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Payment
    {
        [JsonProperty("source")]
        public Source Source { get; set; }
    }
}
