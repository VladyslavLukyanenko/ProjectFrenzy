using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class CheckoutService : ICheckoutService
  {
    private readonly ICheckoutApiClient _checkoutApiClient;
    private readonly ILicenseKeyProvider _licenseKeyProvider;
    private ISourceCache<CheckoutDetailsData, string> _sourceCache;
    private readonly IIdentityService _identityService;

    public CheckoutService(ICheckoutApiClient checkoutApiClient, ILicenseKeyProvider licenseKeyProvider,
      IIdentityService identityService)
    {
      _checkoutApiClient = checkoutApiClient;
      _licenseKeyProvider = licenseKeyProvider;
      _identityService = identityService;

      LastCheckouts = InitializeCache().AsObservableCache();
    }

    public IObservableCache<CheckoutDetailsData, string> LastCheckouts { get; }

    public Task SubmitCheckoutAsync(CheckoutData checkoutData, CancellationToken ct = default)
    {
      return _checkoutApiClient.SubmitCheckoutAsync(checkoutData, _licenseKeyProvider.CurrentLicenseKey, ct);
    }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
      _sourceCache.Clear();
      if (!await _identityService.IsAuthenticated.FirstOrDefaultAsync())
      {
        return;
      }

      var checkouts =
        await _checkoutApiClient.GetLastCheckoutsAsync(_licenseKeyProvider.CurrentLicenseKey, ct);
      _sourceCache.AddOrUpdate(checkouts.Success);
    }

    private IObservable<IChangeSet<CheckoutDetailsData, string>> InitializeCache()
    {
      return ObservableChangeSet.Create<CheckoutDetailsData, string>(async (cache, ct) =>
      {
        _sourceCache = cache;
        await RefreshAsync(ct);
      }, _ => _.Id);
    }
  }
}