using FrenzyBot.Structures.Checkout;
using FrenzyBot.Structures.FlashSale;
using FrenzyBot.Structures.Product;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrenzyBot
{
    class Endpoints
    {
        public static readonly FrenzyHttpClient client = Program.FrenzyHttpClient;
        public async static Task<List<Flashsale>> GetFlashSales()
        {
            var response = await client.GetAsync("https://frenzy.shopifyapps.com/api/flashsales");
            var responseContent = await response.Content.ReadAsStringAsync();
            var list = JObject.Parse(responseContent)["flashsales"].ToString();
            var FlashSaleList = JsonConvert.DeserializeObject<List<Flashsale>>(list, new JsonSerializerSettings{ NullValueHandling = NullValueHandling.Ignore});
            
            //add onboarding sale
            response = await client.GetAsync("https://frenzy.shopifyapps.com/api/flashsales/onboarding_sale");
            responseContent = await response.Content.ReadAsStringAsync();
            var OnboardingFlashale = JsonConvert.DeserializeObject<Flashsale>(JObject.Parse(responseContent)["flashsale"].ToString());
            OnboardingFlashale.Title += " (test item $0.00)";
            FlashSaleList.Add(OnboardingFlashale);

            return FlashSaleList;
        }
        public async static Task<List<Product>> GetProducts(string productId)
        {
            var response = await client.GetAsync($"https://frenzy.shopifyapps.com/api/flashsales/{productId}/products");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            var list = JObject.Parse(responseContent)["products"].ToString();
            return JsonConvert.DeserializeObject<List<Product>>(list);
        }
        public async static Task<Flashsale> GetFlashsale(string password)
        {
            var response = await client.GetAsync($"https://frenzy.shopifyapps.com/api/flashsales/{password}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            var list = JObject.Parse(responseContent)["flashsale"].ToString();
            return JsonConvert.DeserializeObject<Flashsale>(list);
        }
        public async static Task<CheckoutResponse> GetCheckout(string checkoutId)
        {
            var response = await client.GetAsync($"https://frenzy.shopifyapps.com/api/checkouts/{checkoutId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var list = JObject.Parse(responseContent)["checkout"].ToString();
            return JsonConvert.DeserializeObject<CheckoutResponse>(list);
        }
    }
}
