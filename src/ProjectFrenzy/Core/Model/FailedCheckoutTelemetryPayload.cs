using Newtonsoft.Json.Linq;

namespace ProjectFrenzy.Core.Model
{
  public class FailedCheckoutTelemetryPayload
  {
    public CheckoutPayload.CheckoutPayload Payload { get; }
    public JObject RawCheckoutResponse { get; }
    public FrenzyCheckoutTask Task { get; }

    public FailedCheckoutTelemetryPayload(CheckoutPayload.CheckoutPayload payload, JObject rawCheckoutResponse,
      FrenzyCheckoutTask task)
    {
      Payload = payload;
      RawCheckoutResponse = rawCheckoutResponse;
      Task = task;
    }
  }
}