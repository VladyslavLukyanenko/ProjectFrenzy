using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.CheckoutPayload
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Location
    {
        [JsonProperty("altitude")]
        public string Altitude { get; set; }

        [JsonProperty("course")]
        public string Course { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }

        [JsonProperty("speed")]
        public string Speed { get; set; }
    }
}
