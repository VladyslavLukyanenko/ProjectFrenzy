using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Events;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;

namespace ProjectFrenzy.Core.EventHandlers
{
  public class FrenzyCheckoutTaskUnknownErrorsTelemetryEventHandler
    : ApplicationEventHandlerBase<FrenzyCheckoutTaskCompleted>
  {
    private readonly ITelemetryApiClient _telemetryApiClient;
    private readonly ILicenseKeyProvider _licenseKeyProvider;

    public FrenzyCheckoutTaskUnknownErrorsTelemetryEventHandler(ITelemetryApiClient telemetryApiClient,
      ILicenseKeyProvider licenseKeyProvider)
    {
      _telemetryApiClient = telemetryApiClient;
      _licenseKeyProvider = licenseKeyProvider;
    }

    protected override async Task HandleAsync(FrenzyCheckoutTaskCompleted @event, CancellationToken ct)
    {
      if (!@event.Task.Status.IsUnknownError)
      {
        return;
      }

      var raw = JObject.Parse(@event.CheckoutResult.RawResponse);
      var payload = new FailedCheckoutTelemetryPayload(@event.CheckoutPayload, raw, @event.Task);
      await _telemetryApiClient.SubmitFailedCheckoutAsync(payload, _licenseKeyProvider.CurrentLicenseKey, ct);
    }
  }
}