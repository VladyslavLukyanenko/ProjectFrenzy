using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
  public interface ITelemetryApiClient
  {
    Task SubmitFailedCheckoutAsync(FailedCheckoutTelemetryPayload payload, string licenseKey,
      CancellationToken ct = default);
  }
}