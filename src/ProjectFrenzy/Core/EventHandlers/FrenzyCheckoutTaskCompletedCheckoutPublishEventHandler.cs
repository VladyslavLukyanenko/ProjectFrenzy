using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Events;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;

namespace ProjectFrenzy.Core.EventHandlers
{
  public class FrenzyCheckoutTaskCompletedCheckoutPublishEventHandler
    : ApplicationEventHandlerBase<FrenzyCheckoutTaskCompleted>
  {
    private readonly ICheckoutService _checkoutService;

    public FrenzyCheckoutTaskCompletedCheckoutPublishEventHandler(ICheckoutService checkoutService)
    {
      _checkoutService = checkoutService;
    }

    protected override async Task HandleAsync(FrenzyCheckoutTaskCompleted eventMessage, CancellationToken ct)
    {
      var product = eventMessage.Task.Product;
      var checkoutResult = eventMessage.CheckoutResult;
      if (eventMessage.Task.Flashsale.IsTestingItem())
      // if (checkoutResult.TotalPrice <= 0)
      {
        return;
      }

      var checkoutData = new CheckoutData
      {
        Image = product.DefaultPicture,
        Name = product.Title,
        IsSuccess = checkoutResult.Status.IsSuccessful,
        Price = checkoutResult.TotalPrice
      };

      await _checkoutService.SubmitCheckoutAsync(checkoutData, CancellationToken.None);
    }
  }
}