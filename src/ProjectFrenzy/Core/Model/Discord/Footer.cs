using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.Discord
{
    public class Footer
    {
        [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; } =
            "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png";
    }
}
