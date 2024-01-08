using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class LoginViewModel
    : ViewModelBase
  {
    public LoginViewModel(ILicenseKeyProvider licenseKeyProvider, IIdentityService identityService,
      IPreloadService preloadService, ILicenseKeyStore licenseKeyStore,
      IToastNotificationManager toasts)
    {
      WriteLine("Configuring LoginViewModel");
      var isLicenseKeyValid = this.WhenAnyValue(_ => _.LicenseKey)
        .Select(key => !string.IsNullOrEmpty(key));

      var canExecute = new BehaviorSubject<bool>(false);
      LoginCommand = ReactiveCommand.CreateFromTask<Unit>(async (_, ct) =>
      {
        licenseKeyProvider.UseLicenseKey(LicenseKey);
        var authResult = await identityService.FetchIdentityAsync(ct);
        if (authResult.IsSuccess)
        {
          await licenseKeyStore.StoreKeyAsync(LicenseKey, ct);
          await preloadService.PreAuthPreloadAsync(ct);
          identityService.Authenticate(authResult);
          await preloadService.PostAuthPreloadAsync(ct);
        }
        else
        {
            toasts.Show(ToastContent.Error(authResult.Message, "Authentication failed."));
        }
      }, canExecute);
      WriteLine("Created login command");

      WriteLine("Subscribed for check if license if valid");
      isLicenseKeyValid
        .CombineLatest(LoginCommand.IsExecuting, (keyValid, isExecuting) => (keyValid, isExecuting))
        .Select(p => p.keyValid && !p.isExecuting)
        .Subscribe(can => canExecute.OnNext(can));

      LoginCommand.IsExecuting.ToPropertyEx(this, _ => _.IsTryingToLogin);
      RestorePreviouslyUsedKeyCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        WriteLine("Trying to read previous stored key");
        LicenseKey = await licenseKeyStore.GetStoredKeyAsync(ct);
        WriteLine("Previous key restore finished");
      });

      RestorePreviouslyUsedKeyCommand.Execute().Subscribe();

      WriteLine("LoginViewModel is being initialized");
    }
    
    [Reactive] public string LicenseKey { get; set; }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public bool IsTryingToLogin { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> RestorePreviouslyUsedKeyCommand { get; private set; }

    private static void WriteLine(string message)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }
  }
}