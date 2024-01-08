using System.Reflection;
using Newtonsoft.Json;

namespace ProjectFrenzy.Core.Model.Product
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Image
    {
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}
