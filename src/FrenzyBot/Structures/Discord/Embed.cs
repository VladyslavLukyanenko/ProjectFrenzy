using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace FrenzyBot.Structures.Discord
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public partial class Embed
    {
        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("color")]
        public long Color { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("thumbnail")]
        public Image Thumbnail { get; set; }

        [JsonProperty("footer")]
        public Footer Footer { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
    }
}
