using Newtonsoft.Json;
using System.Collections.Generic;
using FrenzyBot.Structures.AndroidResponse;
using System.Reflection;

namespace FrenzyBot.Structures.CheckoutPayload
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class CheckoutPayloadRoot
    {
        [JsonProperty("checkout")]
        public CheckoutPayload Checkout { get; set; }
    }
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class CheckoutPayload
    {
        [JsonProperty("accelerometer_data")]
        public Data AccelerometerData { get; set; }

        [JsonProperty("billing_address")]
        public Address BillingAddress { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("flashsale_password")]
        public string FlashsalePassword { get; set; }

        [JsonProperty("gyro_data")]
        public Data GyroData { get; set; }

        [JsonProperty("last_digits")]
        public string LastDigits { get; set; }

        [JsonProperty("line_items")]
        public List<LineItem> LineItems { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("payment")]
        public Payment Payment { get; set; }

        [JsonProperty("shipping_address")]
        public Address ShippingAddress { get; set; }

        [JsonProperty("shop_id")]
        public long ShopId { get; set; }
    }
}
