using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProjectFrenzy.Core.Model.Product;

namespace ProjectFrenzy.Core.Clients
{
  public class ProductsApiClient : FrenzyApiClientBase, IProductsApiClient
  {
    public async Task<IList<Product>> GetProductByPasswordAsync(string pwd, CancellationToken ct = default)
    {
      while (true)
      {
        try
        {
          var response = await HttpClient.GetAsync($"https://frenzy.shopifyapps.com/api/flashsales/{pwd}/products", ct);
          response.EnsureSuccessStatusCode();
          var responseContent = await response.Content.ReadAsStringAsync();
          return JObject.Parse(responseContent)["products"]?.ToObject<IList<Product>>() ?? Array.Empty<Product>();
        }
        catch (Exception e)
        {
          if ((e is TaskCanceledException || e is OperationCanceledException) && ct.IsCancellationRequested)
          {
            throw;
          }

          await Task.Delay(TimeSpan.FromMilliseconds(50), ct);
        }
      }
    }
  }
}