using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ProjectFrenzy.AvaloniaUI.Views;
using ProjectFrenzy.Core;
using ProjectFrenzy.Core.Services;
using ReactiveUI;
using Splat;

namespace ProjectFrenzy.AvaloniaUI.Infra.Services
{
  public class SecurityManager
    : ISecurityManager
  {
    private readonly IIdentityService _identityService;
    private readonly ILicenseKeyProvider _licenseKeyProvider;
    private readonly SecurityConfig _securityConfig;
    private Window _currentWindow;

    public SecurityManager(IIdentityService identityService, ILicenseKeyProvider licenseKeyProvider,
      SecurityConfig securityConfig)
    {
      _identityService = identityService;
      _licenseKeyProvider = licenseKeyProvider;
      _securityConfig = securityConfig;
    }

    public void Spawn()
    {
      WriteLine("Spawning security manager");
      _identityService.IsAuthenticated
        .DistinctUntilChanged()
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(isAuthenticated =>
        {
          WriteLine("Authentication state changed");
          var lifetime = (IClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime;
          var prevWnd = _currentWindow;
          if (isAuthenticated)
          {
            WriteLine("User is authenticated");
            _currentWindow = Locator.Current.GetService<MainWindowView>();
          }
          else
          {
            WriteLine("User isn't authenticated");
            _currentWindow = Locator.Current.GetService<LoginView>();
          }

          WriteLine("Showing window");
          lifetime.MainWindow = _currentWindow;
          _currentWindow.Show();
          prevWnd?.Close();
          WriteLine("Window shown");
        });

      WriteLine("Started authentication check by interval");
      // var isNotAuthenticated = _identityService.IsAuthenticated.Select(isAuthenticated => !isAuthenticated);
      Observable.Interval(TimeSpan.FromMilliseconds(_securityConfig.ReauthenticateInternalMillis),
          RxApp.TaskpoolScheduler)
        .Subscribe(async _ => { await _identityService.TryAuthenticateAsync(); });
    }

    private static void WriteLine(string message)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }
  }
}