using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Clients;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class UpdateInfoViewModel : ViewModelBase
  {
    private readonly IUpdatesManager _updatesManager;
    private static readonly string InstallerExt = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ".pkg";

    private readonly IUpdateApiClient _updateApiClient;
    private string _installerDirectory;
    private CancellationTokenSource _cts;
    private readonly IToastNotificationManager _toasts;

    public UpdateInfoViewModel(IUpdatesManager updatesManager, IUpdateApiClient updateApiClient, IToastNotificationManager toasts)
    {
      _updatesManager = updatesManager;
      _updateApiClient = updateApiClient;
      _toasts = toasts;
      CheckForUpdatesCommand = ReactiveCommand.CreateFromTask(CheckForUpdates);
      PrepareToUpdateCommand = ReactiveCommand.CreateFromTask(PrepareAsync);
      LaunchUpdaterCommand = ReactiveCommand.Create(LaunchUpdaterAsync);

      updatesManager.AvailableVersion
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.NextVersion);

      var isReadyToUpdate = this.WhenAnyValue(_ => _.State)
        .Select(state => state == States.ReadyToUpdate);

      isReadyToUpdate
        .ToPropertyEx(this, _ => _.IsReadyToUpdate);

      this.WhenAnyValue(_ => _.State)
        .Select(state => state == States.DownloadingUpdateInstaller)
        .ToPropertyEx(this, _ => _.IsInProgress);

      this.WhenAnyValue(_ => _.NextVersion)
        .Select(nextVersion => nextVersion >= AppConstants.CurrentAppVersion)
        .CombineLatest(isReadyToUpdate, (available, isReady) => available && !isReady)
        .ToPropertyEx(this, _ => _.CanDownloadUpdates);

      this.WhenAnyValue(_ => _.NextVersion)
        .Select(nextVersion => nextVersion <= AppConstants.CurrentAppVersion)
        .ToPropertyEx(this, _ => _.AreCurrentVersionLatest);

      var canCancel = this.WhenAnyValue(_ => _.State)
        .Select(s => s == States.DownloadingUpdateInstaller);

      CancelDownloadingCommand = ReactiveCommand.Create(() =>
      {
        _cts?.Cancel();
      }, canCancel);

      Progress = -1;
    }
    
    [Reactive] public int DownloadedMb { get; private set; }
    [Reactive] public int TotalSizeMb { get; private set; }
    
    public bool IsInProgress { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> CancelDownloadingCommand { get; private set; }

    [Reactive] public string CurrentStatus { get; private set; }

    public bool IsReadyToUpdate { [ObservableAsProperty] get; }
    public bool CanDownloadUpdates { [ObservableAsProperty] get; }
    public bool AreCurrentVersionLatest { [ObservableAsProperty] get; }
    
    public Version NextVersion { [ObservableAsProperty] get; }
    [Reactive] public States State { get; private set; }
    [Reactive] public int Progress { get; set; }

    public ReactiveCommand<Unit, Version> CheckForUpdatesCommand { get; private set; }

    public ReactiveCommand<Unit, Unit> PrepareToUpdateCommand { get; private set; }

    public ReactiveCommand<Unit, Unit> LaunchUpdaterCommand { get; private set; }

    private async Task<Version> CheckForUpdates()
    {
      State = States.CheckingForUpdates;
      var nextVersion = await _updatesManager.CheckForUpdatesAsync();
      State = States.CheckedForUpdates;

      var available = nextVersion > AppConstants.CurrentAppVersion;
      if (!available)
      {
        var tc = ToastContent.Information("No updates are available. You are using latest version.");
        _toasts.Show(tc);
        
      }

      return NextVersion;
    }

    private async Task PrepareAsync()
    {
      _cts = new CancellationTokenSource();
      State = States.DownloadingUpdateInstaller;
      CurrentStatus = "Downloading v" + NextVersion;
      
      
      _installerDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(_installerDirectory);
      var installerFileName = NextVersion + InstallerExt;

      var installerFullPath = Path.Combine(_installerDirectory, installerFileName);
      using (var file = File.Create(installerFullPath))
      {
        try
        {
          await _updateApiClient.DownloadInstallerAsync(file, NextVersion, Environment.Is64BitOperatingSystem,
            (totalBytes, downloadedBytes, calculatedProgressPercents) =>
            {
              TotalSizeMb = (int) (totalBytes / 1024 / 1024);
              DownloadedMb = (int) (downloadedBytes / 1024 / 1024);
              Progress = calculatedProgressPercents;
            }, _cts.Token);
        }
        catch (OperationCanceledException)
        {
          Directory.Delete(_installerDirectory, true);
          await CheckForUpdates();
          // expected
          return;
        }
      }

      State = States.ReadyToUpdate;
      CurrentStatus = "Starting update";
      LaunchUpdaterCommand.Execute().Subscribe();
    }

    private void LaunchUpdaterAsync()
    {
      var installerFullPath = Path.Combine(_installerDirectory, $"{NextVersion}" + InstallerExt);
      StartInstaller(installerFullPath);
    }
    
    
    private Process StartInstaller(string installerFullPath)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        return Process.Start(installerFullPath);
      }
      else
      {
        var process = Process.Start("open", installerFullPath);
        Thread.Sleep(1000);
        return process;
      }
    }
    
    public enum States
    {
      CheckingForUpdates,
      CheckedForUpdates,
      DownloadingUpdateInstaller,
      ReadyToUpdate,
    }
  }
}
