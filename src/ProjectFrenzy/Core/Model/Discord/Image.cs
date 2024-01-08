using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.Discord
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Image
    {
        [JsonProperty("url")] public string Url { get; set; } = "";
    }
}
