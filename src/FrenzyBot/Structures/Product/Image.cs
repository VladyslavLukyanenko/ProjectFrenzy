using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.Product
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Image
    {
        [JsonProperty("src")]
        public string Src { get; set; }
    }
}
