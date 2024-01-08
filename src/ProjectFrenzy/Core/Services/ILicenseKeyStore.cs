using System.Threading;
using System.Threading.Tasks;

namespace ProjectFrenzy.Core.Services
{
  public interface ILicenseKeyStore
  {
    Task<string> GetStoredKeyAsync(CancellationToken ct = default);
    Task StoreKeyAsync(string key, CancellationToken ct = default);
  }
}