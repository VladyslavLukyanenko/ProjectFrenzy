using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
  public interface ICountriesApiClient
  {
    Task<IList<Country>> GetCountriesAsync(string licenseKey, CancellationToken ct = default);
  }
}