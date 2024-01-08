﻿using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.CheckoutPayload
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class PaymentToken
    {
        [JsonProperty("payment_data")]
        public string PaymentData { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}