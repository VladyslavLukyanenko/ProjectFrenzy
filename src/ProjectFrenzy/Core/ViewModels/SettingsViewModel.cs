using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ToastNotifications;
using ProjectFrenzy.Core.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class SettingsViewModel
    : ViewModelBase, IRoutableViewModel
  {
    private readonly ReadOnlyObservableCollection<EmulatorRowViewModel> _emulators;

    public SettingsViewModel(IEmulatorService emulatorService, IDiscordSettingsService discordSettingsService,
      IScreen hostScreen, DiscordSettingsValidator discordSettingsValidator, IToastNotificationManager toasts,
      EmulatorValidator emulatorValidator, IWebHookManager webHookManager,
      IEmulatorsAvailabilityWatcher emulatorsAvailabilityWatcher, UpdateInfoViewModel updateInfo)
    {
      HostScreen = hostScreen;

      emulatorService.Emulators.Connect()
        .ObserveOn(RxApp.MainThreadScheduler)
        .Transform(e => new EmulatorRowViewModel(e))
        .Bind(out _emulators)
        .DisposeMany()
        .Subscribe();

      discordSettingsService.Settings.ToPropertyEx(this, _ => _.DiscordSettings);

      UpdateDiscordSettingsCommand =
        ReactiveCommand.CreateFromTask(async ct =>
        {
          var result = await discordSettingsValidator.ValidateAsync(DiscordSettings);
          if (!result.IsValid)
          {
            toasts.Show(ToastContent.Error(result.ToString()));

            return;
          }

          await discordSettingsService.UpdateAsync(DiscordSettings, ct);
          toasts.Show(ToastContent.Success("Webhooks saved"));
        });


      AddEmulatorCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        if (!emulatorValidator.IsEndpointValid(EmulatorIp))
        {
          toasts.Show(ToastContent.Error(
            "Invalid emulator endpoint provided provided. It can be IP address or ngrok tunneled URL."));
          return;
        }

        var emulator = Emulator.FromRawEndpointAddress(EmulatorIp);
        await emulatorsAvailabilityWatcher.UpdateAvailabilityAsync(emulator, ct);
        if (!emulator.IsAvailable)
        {
          toasts.Show(ToastContent.Warning(
            "This emulator is not accessible right now. Please check if you started it and configured properly."));
        }

        await emulatorService.AddOrUpdateAsync(emulator, ct);
        EmulatorIp = null;
        toasts.Show(ToastContent.Success("Emulator added"));
      });

      RemoveEmulatorCommand = ReactiveCommand.CreateFromTask<EmulatorRowViewModel>(async (e, ct) =>
      {
        await emulatorService.RemoveAsync(e.Emulator, ct);
        toasts.Show(ToastContent.Success("Emulator removed"));
      });

      SendTestWebhookCommand = ReactiveCommand.CreateFromTask(async () =>
      {
        ToastContent content;
        if (await webHookManager.TestWebhook())
        {
          content = ToastContent.Success("Test webhook sent successfully");
        }
        else
        {
          content = ToastContent.Error("Webhook wasn't sent. Please check urls provided");
        }

        toasts.Show(content);
      });
      UpdateInfo = updateInfo;
    }

    public UpdateInfoViewModel UpdateInfo { get; private set; }
    [Reactive] public string EmulatorIp { get; set; }
    public ReactiveCommand<Unit, Unit> AddEmulatorCommand { get; set; }
    public ReactiveCommand<EmulatorRowViewModel, Unit> RemoveEmulatorCommand { get; set; }
    public DiscordSettings DiscordSettings { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> UpdateDiscordSettingsCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SendTestWebhookCommand { get; set; }


    public ReadOnlyObservableCollection<EmulatorRowViewModel> Emulators => _emulators;
    public string UrlPathSegment => nameof(SettingsViewModel);
    public IScreen HostScreen { get; }
  }
}