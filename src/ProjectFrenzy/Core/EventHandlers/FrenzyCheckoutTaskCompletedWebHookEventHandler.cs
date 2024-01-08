using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Events;
using ProjectFrenzy.Core.Services;

namespace ProjectFrenzy.Core.EventHandlers
{
  public class FrenzyCheckoutTaskCompletedWebHookEventHandler : ApplicationEventHandlerBase<FrenzyCheckoutTaskCompleted>
  {
    private readonly IWebHookManager _webHookManager;

    public FrenzyCheckoutTaskCompletedWebHookEventHandler(IWebHookManager webHookManager)
    {
      _webHookManager = webHookManager;
    }

    protected override Task HandleAsync(FrenzyCheckoutTaskCompleted eventMessage, CancellationToken ct)
    {
      _webHookManager.EnqueueWebhook(eventMessage.CheckoutResult, eventMessage.Task);
      return Task.CompletedTask;
    }
  }
}