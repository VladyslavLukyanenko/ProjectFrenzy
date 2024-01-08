using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
    public interface IStatsService
    {
        IObservable<FrenzyStatisticsResponseModel> Stats { get; }
        Task RefreshAsync(CancellationToken ct = default);
        Task<bool> SubmitStatsAsync(CheckoutStats stats, CancellationToken ct = default);
    }
}