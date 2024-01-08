using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProjectFrenzy.Core.Model.FlashSale;

namespace ProjectFrenzy.Core.Clients
{
  public class FlashsalesApiClient : FrenzyApiClientBase, IFlashsalesApiClient
  {
    public async Task<IList<Flashsale>> GetAllAsync(CancellationToken ct = default)
    {
      var flashSaleList = new List<Flashsale>();

      //add onboarding sale
      var response = await HttpClient.GetAsync("https://frenzy.shopifyapps.com/api/flashsales/onboarding_sale", ct);
      var responseContent = await response.Content.ReadAsStringAsync();
      var onboardingFlashale = JObject.Parse(responseContent)["flashsale"]?.ToObject<Flashsale>();
      if (onboardingFlashale != null)
      {
        onboardingFlashale.Title += " (test item $0.00)";
        onboardingFlashale.TestSale = true;
        flashSaleList.Add(onboardingFlashale);
      }

      response = await HttpClient.GetAsync("https://frenzy.shopifyapps.com/api/flashsales", ct);
      responseContent = await response.Content.ReadAsStringAsync();
      var r = JObject.Parse(responseContent)["flashsales"]?.ToObject<List<Flashsale>>() ??
              new List<Flashsale>();

      flashSaleList.AddRange(r);

      return flashSaleList;
    }

    public async Task<Flashsale> GetByPasswordAsync(string password, CancellationToken ct = default)
    {
      var response = await HttpClient.GetAsync($"https://frenzy.shopifyapps.com/api/flashsales/{password}", ct);
      var responseContent = await response.Content.ReadAsStringAsync();

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      return JObject.Parse(responseContent)["flashsale"]?.ToObject<Flashsale>();
    }
  }
}