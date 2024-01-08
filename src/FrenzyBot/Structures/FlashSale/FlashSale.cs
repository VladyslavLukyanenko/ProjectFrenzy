using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FrenzyBot.Structures.FlashSale
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public partial class Flashsale
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("available_at")]
        public DateTimeOffset AvailableAt { get; set; }

        [JsonProperty("started_at")]
        public DateTimeOffset StartedAt { get; set; }

        [JsonProperty("ended_at")]
        public DateTimeOffset EndedAt { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("sold_out")]
        public bool SoldOut { get; set; }

        [JsonProperty("products_count")]
        public long ProductsCount { get; set; }

        [JsonProperty("custom_message")]
        public string CustomMessage { get; set; }

        [JsonProperty("dropzone")]
        public List<Dropzone> Dropzone { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price_range")]
        public PriceRange PriceRange { get; set; }

        [JsonProperty("image_urls")]
        public List<Uri> ImageUrls { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("global_visibility")]
        public bool GlobalVisibility { get; set; }

        [JsonProperty("test_sale")]
        public bool TestSale { get; set; }

        [JsonProperty("shipping_message")]
        public string ShippingMessage { get; set; }

        [JsonProperty("interactive_sale")]
        public bool InteractiveSale { get; set; }

        [JsonProperty("product_details")]
        public List<ProductDetail> ProductDetails { get; set; }

        [JsonProperty("deal_sale")]
        public bool DealSale { get; set; }

        [JsonProperty("pickup")]
        public bool Pickup { get; set; }

        [JsonProperty("shop")]
        public Shop Shop { get; set; }

        public bool Upcoming { get; set; }

    }
}
