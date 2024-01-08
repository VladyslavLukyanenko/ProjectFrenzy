using System.Threading;
using System.Threading.Tasks;

namespace ProjectFrenzy.Core.Clients
{
    public interface ILicenseKeyApiClient
    {
        Task<AuthenticationResult> Authenticate(string licenseKey, string hwid, CancellationToken ct = default);
    }
}