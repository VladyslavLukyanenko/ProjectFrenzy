using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.Discord
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Author
    {
        [JsonProperty("name")] public string Name { get; set; } = "";

        [JsonProperty("url")] public string Url { get; set; } = "";

        [JsonProperty("icon_url")] public string IconUrl { get; set; } = "https://cdn.discordapp.com/attachments/682673305995575370/685699712086441993/ProjectFrenzy.png";
    }
}
