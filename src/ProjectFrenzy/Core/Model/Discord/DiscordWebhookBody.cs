using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.Discord
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class DiscordWebhookBody
    {
        [JsonProperty("content")] public string Content { get; set; } = "";

        [JsonProperty("username")] public string Username { get; set; } = "Project Frenzy";

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; } =
            "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png";

        [JsonProperty("embeds")] public List<Embed> Embeds { get; set; } = new List<Embed>();
    }
}
