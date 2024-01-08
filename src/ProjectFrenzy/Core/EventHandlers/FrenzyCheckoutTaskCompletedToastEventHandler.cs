using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Events;
using ProjectFrenzy.Core.ToastNotifications;

namespace ProjectFrenzy.Core.EventHandlers
{
  public class FrenzyCheckoutTaskCompletedToastEventHandler : ApplicationEventHandlerBase<FrenzyCheckoutTaskCompleted>
  {
    private readonly IToastNotificationManager _toasts;

    public FrenzyCheckoutTaskCompletedToastEventHandler(IToastNotificationManager toasts)
    {
      _toasts = toasts;
    }

    protected override Task HandleAsync(FrenzyCheckoutTaskCompleted @event, CancellationToken ct)
    {
      var task = @event.Task;
      var checkoutResult = @event.CheckoutResult;
      var isSuccessful = checkoutResult.Status.IsSuccessful;
      
      var builder = new StringBuilder();
      builder.AppendLine($"Email: {task.AssignedEmail.Value}");
      builder.AppendLine($"Product: {task.Product.Title}");
      builder.AppendLine($"Mode: {task.Mode}");
      builder.AppendLine($"Shop: {task.Flashsale.Shop.Name}");
      var priceOrErrorTitle = isSuccessful ? "Price" : "Error";
      var priceOrErrorValue = isSuccessful
        ? checkoutResult.TotalPrice.ToString(CultureInfo.InvariantCulture)
        : checkoutResult.StatusMessage;

      builder.AppendLine($"{priceOrErrorTitle}: {priceOrErrorValue}");
      builder.AppendLine($"Option: {string.Join(",", task.SelectedSizes)}");
      builder.AppendLine($"Checkout Time: {task.CheckoutDuration.Seconds}.{task.CheckoutDuration.Milliseconds:D3} seconds");
      builder.AppendLine($"Delay: {task.CheckoutDelay}");

      var message = builder.ToString();
      var status = isSuccessful ? "Successful" : "Failed";
      var title = "Checkout " + status;
      ToastContent content = isSuccessful ? ToastContent.Success(message, title) : ToastContent.Error(message, title); 
      _toasts.Show(content);
      return Task.CompletedTask;
    }
  }
}