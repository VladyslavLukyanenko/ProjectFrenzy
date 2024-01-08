using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.FlashSale
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Dropzone
    {
        [JsonProperty("lng")]
        public double Lng { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }
    }
}
