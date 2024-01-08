using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
  public class TelemetryApiClient : ApiClientBase, ITelemetryApiClient
  {
    public TelemetryApiClient(ProjectIndustriesApiConfig apiConfig) : base(apiConfig)
    {
    }

    public Task SubmitFailedCheckoutAsync(FailedCheckoutTelemetryPayload payload, string licenseKey,
      CancellationToken ct = default)
    {
      return PostAsync("/telemetry", licenseKey, payload, ct);
    }
  }
}