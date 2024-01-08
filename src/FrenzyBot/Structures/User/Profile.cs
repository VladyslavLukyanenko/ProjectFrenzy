using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.User
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Profile
    {
        [JsonProperty("ProfileName")]
        public string ProfileName { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("ShippingFirstName")]
        public string ShippingFirstName { get; set; }

        [JsonProperty("ShippingLastName")]
        public string ShippingLastName { get; set; }

        [JsonProperty("ShippingAddress1")]
        public string ShippingAddress1 { get; set; }

        [JsonProperty("ShippingAddress2")]
        public string ShippingAddress2 { get; set; }

        [JsonProperty("ShippingCity")]
        public string ShippingCity { get; set; }

        [JsonProperty("ShippingZip")]
        public string ShippingZip { get; set; }

        [JsonProperty("ShippingCountry")]
        public string ShippingCountry { get; set; }

        [JsonProperty("ShippingState")]
        public string ShippingState { get; set; }

        [JsonProperty("UseBilling")]
        public bool UseBilling { get; set; }

        [JsonProperty("BillingFirstName")]
        public string BillingFirstName { get; set; }

        [JsonProperty("BillingLastName")]
        public string BillingLastName { get; set; }

        [JsonProperty("BillingAddress1")]
        public string BillingAddress1 { get; set; }

        [JsonProperty("BillingAddress2")]
        public string BillingAddress2 { get; set; }

        [JsonProperty("BillingCity")]
        public string BillingCity { get; set; }

        [JsonProperty("BillingZip")]
        public string BillingZip { get; set; }

        [JsonProperty("BillingCountry")]
        public string BillingCountry { get; set; }

        [JsonProperty("BillingState")]
        public string BillingState { get; set; }
    }
}
