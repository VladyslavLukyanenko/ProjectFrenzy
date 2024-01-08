using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.CheckoutPayload
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Source
    {
        [JsonProperty("payment_token")]
        public PaymentToken PaymentToken { get; set; }
    }
}
