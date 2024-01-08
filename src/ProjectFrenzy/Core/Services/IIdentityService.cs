using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
    public interface IIdentityService
    {
        IObservable<User> CurrentUser { get; }
        IObservable<bool> IsAuthenticated { get; }
        Task<AuthenticationResult> TryAuthenticateAsync(CancellationToken ct = default);
        Task<AuthenticationResult> FetchIdentityAsync(CancellationToken ct = default);
        void Authenticate(AuthenticationResult result);
    }
}