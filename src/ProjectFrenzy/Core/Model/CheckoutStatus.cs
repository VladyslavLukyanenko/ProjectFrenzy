using System;

namespace ProjectFrenzy.Core.Model
{
  public abstract class CheckoutStatus : Enumeration
  {
    public static readonly CheckoutStatus Created = new CreatedStatus(1, nameof(Created), "Ready to start");

    // public static readonly CheckoutStatus PendingAuthorization = new RunningStatus(2, "Pending Authorization");
    public static readonly CheckoutStatus EmulatorSelected = new RunningStatus(2, "Emulator Selected. Authorizing...");

    public static readonly CheckoutStatus AwaitingForFreeEmulator =
      new RunningStatus(2, "Awaiting for free Emulator...");

    public static readonly CheckoutStatus SelectingProductVariant =
      new RunningStatus(3, "Selecting Product Variant");

    public static readonly CheckoutStatus FetchingProduct = new RunningStatus(3, "Fetching Product");

    public static readonly CheckoutStatus Submitting = new RunningStatus(6, "Submitting");
    public static readonly CheckoutStatus ProcessingResults = new RunningStatus(7, "Processing Results");

    public static readonly CheckoutStatus CheckoutSuccessful = new SuccessfulStatus(8, "Checkout Successful");

//checkoutTime.ToString(@"ss\.fff") + " seconds";
    public static readonly CheckoutStatus VariantNotFound = new FailedStatus(10, "Variant Not Found");
    public static readonly CheckoutStatus NoEmulatorAvailable = new FailedStatus(11, "No Emulator Available");
    public static readonly CheckoutStatus NotEmailAvailable = new FailedStatus(11, "No Email Available");
    public static readonly CheckoutStatus NoProxiesConfigured = new FailedStatus(12, "No Proxies Configured");
    public static readonly CheckoutStatus PaymentDeclined = new FailedStatus(13, "Payment Declined");
    public static readonly CheckoutStatus OutOfStock = new FailedStatus(14, "Out of Stock");

    public static readonly CheckoutStatus Cancelled = new CancelledStatus(16, nameof(Cancelled));

    public string HexColor { get; private set; }
    public string Description { get; private set; }
    public bool IsUnknownError { get; }

    protected CheckoutStatus(int id, string name, string hexColor, string description = null,
      bool isUnknownError = false) : base(id, name)
    {
      HexColor = hexColor;
      Description = description;
      IsUnknownError = isUnknownError;
    }

    public bool IsRunning => GetType() == typeof(RunningStatus);
    public bool IsSuccessful => GetType() == typeof(SuccessfulStatus);
    public bool IsFailed => GetType() == typeof(FailedStatus);


    public static CheckoutStatus RetryAuthorization(int attempt) =>
      new RunningStatus(2, $"Retrying to Authorize {attempt}...");

    public static CheckoutStatus AwaitingForUpcoming(TimeSpan timeLeft) =>
      new RunningStatus(4, timeLeft.ToHumanReadableString(), "Awaiting for Drop");

    public static CheckoutStatus DelayUntilCheckoutTime(TimeSpan timeLeft) =>
      new RunningStatus(5, timeLeft.ToHumanReadableString(), "Until Checkout");

    public static CheckoutStatus FailedCheckout(string desc) => new FailedStatus(9, "Failed Checkout", desc, true);
    public static CheckoutStatus UnknownError(string desc) => new FailedStatus(15, "Unknown Error", desc, true);


    private class CreatedStatus : CheckoutStatus
    {
      public CreatedStatus(int id, string name, string desc = null) : base(id, name, "#696969", desc)
      {
      }
    }

    private class RunningStatus : CheckoutStatus
    {
      public RunningStatus(int id, string name, string description = null) : base(id, name, "#6495ed", description)
      {
      }
    }

    private class SuccessfulStatus : CheckoutStatus
    {
      public SuccessfulStatus(int id, string name, string desc = null)
        : base(id, name, "#00FFA2", desc)
      {
      }
    }

    private class FailedStatus : CheckoutStatus
    {
      public FailedStatus(int id, string name, string desc = null, bool isUnknownError = false)
        : base(id, name, "#FC2A57", desc, isUnknownError)
      {
      }
    }

    private class CancelledStatus : CheckoutStatus
    {
      public CancelledStatus(int id, string name) : base(id, name, "#ffa500")
      {
      }
    }
  }
}