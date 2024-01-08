using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.AvaloniaUI.Controls
{
  public class CheckoutStatusTextBlock : TextBlock
  {
    // private static readonly IDictionary<string, CheckoutStatus[]> SupportedPseudoClasses =
    //   new Dictionary<string, CheckoutStatus[]>
    //   {
    //     {":created", new[] {CheckoutStatus.Created}},
    //     {
    //       ":running",
    //       new[]
    //       {
    //         CheckoutStatus.Running,
    //         CheckoutStatus.SelectingProductVariant,
    //         CheckoutStatus.PendingAuthorization,
    //         CheckoutStatus.AwaitingForUpcoming,
    //         CheckoutStatus.DelayUntilCheckoutTime,
    //         CheckoutStatus.Submitting,
    //         CheckoutStatus.ProcessingResults,
    //       }
    //     },
    //     {
    //       ":failed",
    //       new[]
    //       {
    //         CheckoutStatus.FailedCheckout,
    //         CheckoutStatus.VariantNotFound,
    //         CheckoutStatus.NoEmulatorAvailable,
    //         CheckoutStatus.NoProxiesConfigured,
    //         CheckoutStatus.PaymentDeclined,
    //         CheckoutStatus.OutOfStock,
    //         CheckoutStatus.UnknownError
    //       }
    //     },
    //     {
    //       ":cancelled",
    //       new[]
    //       {
    //         CheckoutStatus.Cancelled
    //       }
    //     },
    //     {
    //       ":successful",
    //       new[]
    //       {
    //         CheckoutStatus.CheckoutSuccessful
    //       }
    //     }
    //   };
    //
    // private static readonly IDictionary<CheckoutStatus, string> Status2PseudoClassesCache =
    //   SupportedPseudoClasses
    //     .SelectMany(p => p.Value.Select(v => (Cls: p.Key, Status: v)))
    //     .ToDictionary(_ => _.Status, _ => _.Cls);

    
    public static readonly DirectProperty<CheckoutStatusTextBlock, CheckoutStatus> StatusProperty =
      AvaloniaProperty.RegisterDirect<CheckoutStatusTextBlock, CheckoutStatus>(
        nameof(Status),
        o => o.Status,
        (o, v) => o.Status = v, CheckoutStatus.Created);


    private CheckoutStatus _status;
    public CheckoutStatus Status
    {
      get => _status;
      set
      {
        SetAndRaise(StatusProperty, ref _status, value);
        Foreground = SolidColorBrush.Parse(value.HexColor);
        Text = value.Name;
      }
    }
  }
}