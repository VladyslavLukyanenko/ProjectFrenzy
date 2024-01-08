using Newtonsoft.Json;
using System.Reflection;

namespace FrenzyBot.Structures.AndroidResponse
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Address
    {
        [JsonProperty("address1")]
        public string Address1 { get; set; }

        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }
    }
}
