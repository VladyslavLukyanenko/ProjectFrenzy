using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Clients;

namespace ProjectFrenzy.Core.Services
{
  public interface ICheckoutService
  {
    IObservableCache<CheckoutDetailsData, string> LastCheckouts { get; }
    Task SubmitCheckoutAsync(CheckoutData checkoutData, CancellationToken ct = default);
    Task RefreshAsync(CancellationToken ct = default);
  }
}