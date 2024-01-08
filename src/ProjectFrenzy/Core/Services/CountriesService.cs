using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class CountriesService
    : ICountriesService
  {
    private readonly ILicenseKeyProvider _licenseKeyProvider;
    private readonly ICountriesApiClient _countriesApiClient;

    public CountriesService(ILicenseKeyProvider licenseKeyProvider, ICountriesApiClient countriesApiClient)
    {
      _licenseKeyProvider = licenseKeyProvider;
      _countriesApiClient = countriesApiClient;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
      var countries = await _countriesApiClient.GetCountriesAsync(_licenseKeyProvider.CurrentLicenseKey, ct);
      Countries = countries.ToList();
    }

    public IReadOnlyList<Country> Countries { get; private set; } = new List<Country>();
  }
}