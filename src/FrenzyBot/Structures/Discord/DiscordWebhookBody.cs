using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;

namespace FrenzyBot.Structures.Discord
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class DiscordWebhookBody
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("embeds")]
        public List<Embed> Embeds { get; set; }
    }
}
