using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class StatsService : IStatsService
  {
    private readonly IFrenzyApiClient _frenzyApiClient;
    private readonly ILicenseKeyProvider _licenseKeyProvider;

    private readonly BehaviorSubject<FrenzyStatisticsResponseModel> _stats =
      new BehaviorSubject<FrenzyStatisticsResponseModel>(null);

    private readonly IStatsApiClient _statsApiClient;

    public StatsService(IFrenzyApiClient frenzyApiClient, ILicenseKeyProvider licenseKeyProvider,
      IStatsApiClient statsApiClient)
    {
      _frenzyApiClient = frenzyApiClient;
      _licenseKeyProvider = licenseKeyProvider;
      _statsApiClient = statsApiClient;
      Stats = _stats;
    }

    public IObservable<FrenzyStatisticsResponseModel> Stats { get; }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
      var stats = await _frenzyApiClient.GetStatsAsync(_licenseKeyProvider.CurrentLicenseKey, ct);
      _stats.OnNext(stats);
    }

    public Task<bool> SubmitStatsAsync(CheckoutStats stats, CancellationToken ct = default)
    {
      return _statsApiClient.SubmitStatsAsync(stats, _licenseKeyProvider.CurrentLicenseKey, ct);
    }
  }
}