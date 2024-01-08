using System.Threading;
using System.Threading.Tasks;

namespace ProjectFrenzy.Core.Services
{
    public interface IDeviceInfoProvider
    {
        Task<string> GetHwidAsync(CancellationToken ct);
    }
}