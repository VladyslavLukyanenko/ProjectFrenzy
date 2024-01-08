using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Model.CheckoutPayload;

namespace ProjectFrenzy.Core.Events
{
  public class FrenzyCheckoutTaskCompleted
  {
    public FrenzyCheckoutTaskCompleted(FrenzyCheckoutTask task, CheckoutResult checkoutResult, CheckoutPayload checkoutPayload)
    {
      Task = task;
      CheckoutResult = checkoutResult;
      CheckoutPayload = checkoutPayload;
    }

    public FrenzyCheckoutTask Task { get; }
    public CheckoutResult CheckoutResult { get; }
    public CheckoutPayload CheckoutPayload { get; }
  }
}