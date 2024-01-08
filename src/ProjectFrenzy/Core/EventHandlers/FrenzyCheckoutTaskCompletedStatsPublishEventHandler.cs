using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Events;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;

namespace ProjectFrenzy.Core.EventHandlers
{
  public class FrenzyCheckoutTaskCompletedStatsPublishEventHandler
    : ApplicationEventHandlerBase<FrenzyCheckoutTaskCompleted>
  {
    private readonly IStatsService _statsService;

    public FrenzyCheckoutTaskCompletedStatsPublishEventHandler(IStatsService statsService)
    {
      _statsService = statsService;
    }

    protected override async Task HandleAsync(FrenzyCheckoutTaskCompleted eventMessage, CancellationToken ct)
    {
      if (eventMessage.Task.Flashsale.IsTestingItem() || eventMessage.CheckoutResult.Status.IsFailed)
      // if (eventMessage.CheckoutResult.TotalPrice <= 0 || eventMessage.CheckoutResult.Status.IsFailed)
      {
        return;
      }

      var stats = CheckoutStats.Create(eventMessage.CheckoutResult);
      await _statsService.SubmitStatsAsync(stats, ct);
    }
  }
}