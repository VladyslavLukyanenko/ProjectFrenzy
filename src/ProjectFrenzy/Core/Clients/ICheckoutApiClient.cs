using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
  public interface ICheckoutApiClient
  {
    Task<LastCheckoutsResponse> GetLastCheckoutsAsync(string licenseKey, CancellationToken ct = default);
    Task<bool> SubmitCheckoutAsync(CheckoutData data, string licenseKey, CancellationToken ct = default);
  }
}