using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Model;
using ReactiveUI;

namespace ProjectFrenzy.Core.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly ILicenseKeyApiClient _licenseKeyApiClient;
        private readonly IDeviceInfoProvider _deviceInfoProvider;
        private readonly ILicenseKeyProvider _licenseKeyProvider;
        private readonly ISoftwareInfoProvider _softwareInfoProvider;
        private readonly BehaviorSubject<User> _currentUser = new BehaviorSubject<User>(null);

        public IdentityService(ILicenseKeyApiClient licenseKeyApiClient, IDeviceInfoProvider deviceInfoProvider,
            ILicenseKeyProvider licenseKeyProvider, ISoftwareInfoProvider softwareInfoProvider)
        {
            _licenseKeyApiClient = licenseKeyApiClient;
            _deviceInfoProvider = deviceInfoProvider;
            _licenseKeyProvider = licenseKeyProvider;
            _softwareInfoProvider = softwareInfoProvider;

            CurrentUser = _currentUser
                .ObserveOn(RxApp.MainThreadScheduler);
            
            IsAuthenticated = CurrentUser.Select(user => user != null);
        }

        public IObservable<User> CurrentUser { get; }
        public IObservable<bool> IsAuthenticated { get; }

        public async Task<AuthenticationResult> TryAuthenticateAsync(CancellationToken ct = default)
        {
            var result = await FetchIdentityAsync(ct);
            if (result.IsSuccess)
            {
                _softwareInfoProvider.SetSoftwareVersion(result.SoftwareVersion);
                var expiryDate = result.Expiry.HasValue 
                    ? (DateTimeOffset?) DateTimeOffset.FromUnixTimeSeconds(result.Expiry.Value)
                    : null;

                var user = new User(result.DiscordId, result.UserName, result.Discriminator, expiryDate, result.Avatar);
                _currentUser.OnNext(user);
            }
            else
            {
                Invalidate();
            }

            return result;
        }

        public async Task<AuthenticationResult> FetchIdentityAsync(CancellationToken ct = default)
        {
            var licenseKey = _licenseKeyProvider.CurrentLicenseKey;
            if (string.IsNullOrEmpty(licenseKey))
            {
                return AuthenticationResult.CreateUnkownError();
            }

            var hwid = await _deviceInfoProvider.GetHwidAsync(ct);
            return await _licenseKeyApiClient.Authenticate(licenseKey, hwid, ct);
        }

        public void Authenticate(AuthenticationResult result)
        {
            if (result.IsSuccess)
            {
                _softwareInfoProvider.SetSoftwareVersion(result.SoftwareVersion);
                var expiryDate = result.Expiry.HasValue 
                    ? (DateTimeOffset?) DateTimeOffset.FromUnixTimeSeconds(result.Expiry.Value)
                    : null;

                var user = new User(result.DiscordId, result.UserName, result.Discriminator, expiryDate, result.Avatar);
                _currentUser.OnNext(user);
            }
            else
            {
                Invalidate();
            }
        }

        private void Invalidate()
        {
            _licenseKeyProvider.Invalidate();
            _currentUser.OnNext(null);
        }
    }
}