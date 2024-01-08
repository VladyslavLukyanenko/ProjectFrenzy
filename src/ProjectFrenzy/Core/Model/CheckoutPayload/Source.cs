using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.CheckoutPayload
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Source
    {
        [JsonProperty("payment_token")]
        public PaymentToken PaymentToken { get; set; }
    }
}
