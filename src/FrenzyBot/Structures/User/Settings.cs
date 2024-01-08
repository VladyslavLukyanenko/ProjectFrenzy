using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.User
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Settings
    {
        [JsonProperty("DiscordWebhookUrl")]
        public string DiscordWebhookUrl { get; set; }

        [JsonProperty("LicenseKey")]
        public string LicenseKey { get; set; }

        [JsonProperty("EmualtorIp")]
        public string EmulatorIp { get; set; }
    }
}
