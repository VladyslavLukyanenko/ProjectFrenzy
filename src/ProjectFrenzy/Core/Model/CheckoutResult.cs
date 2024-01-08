namespace ProjectFrenzy.Core.Model
{
  public class CheckoutResult
  {
    public CheckoutStatus Status { get; set; }
    public string StatusMessage { get; set; }
    public decimal TotalPrice { get; set; }
    public string RawResponse { get; set; }
  }
}