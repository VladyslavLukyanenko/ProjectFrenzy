using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
    public interface IFrenzyApiClient
    {
        Task<string> DecodeAsync(string licenseKey, string content, CancellationToken ct = default);
        Task<FrenzyStatisticsResponseModel> GetStatsAsync(string licenseKey, CancellationToken ct = default);
    }
}