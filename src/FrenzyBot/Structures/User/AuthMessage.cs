using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.User
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class AuthMessage
    {
        [JsonProperty("LicenseKey")]
        public string LicenseKey { get; set; }

        [JsonProperty("SessionId")]
        public string SessionId { get; set; }
        
        [JsonProperty("InitialCheck")]
        public bool InitialCheck { get; set; }

        [JsonProperty("Version")]
        public string Version { get; set; }

        [JsonProperty("Timestamp")]
        public long Timestamp { get; set; }
    }
}
