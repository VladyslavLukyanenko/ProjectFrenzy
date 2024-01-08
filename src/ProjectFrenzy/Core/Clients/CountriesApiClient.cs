using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
  public class CountriesApiClient : ApiClientBase, ICountriesApiClient
  {
    public CountriesApiClient(ProjectIndustriesApiConfig apiConfig) : base(apiConfig)
    {
    }

    public Task<IList<Country>> GetCountriesAsync(string licenseKey, CancellationToken ct = default)
    {
      return GetAsync<IList<Country>>("/countries", licenseKey, ct);
    }
  }
}