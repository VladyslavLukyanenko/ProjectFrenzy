﻿using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.AndroidResponse
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class AndroidResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("card")]
        public string Card { get; set; }

        [JsonProperty("address")]
        public Addresses Address { get; set; }
    }
}
