using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Clients
{
    public interface IStatsApiClient
    {
        Task<bool> SubmitStatsAsync(CheckoutStats stats, string licenseKey, CancellationToken ct = default);
    }
}