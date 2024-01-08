// using System;
// using System.Diagnostics;
// using System.IO;
// using System.Linq;
// using System.Reactive;
// using System.Reactive.Linq;
// using System.Threading.Tasks;
// using ProjectFrenzy.Core.Clients;
// using ProjectFrenzy.Core.Services;
// using ReactiveUI;
// using ReactiveUI.Fody.Helpers;
//
// namespace ProjectFrenzy.Core.ViewModels
// {
//   public class UpdatesWindowViewModel
//     : ViewModelBase
//   {
//     private const string UpdaterExecutableName = "ProjectFrenzy.Updater.exe";
//     private readonly IUpdateApiClient _updateApiClient;
//     private string _installerDirectory;
//     private readonly ILicenseKeyProvider _licenseKeyProvider;
//
//     public UpdatesWindowViewModel(IUpdateApiClient updateApiClient, ILicenseKeyProvider licenseKeyProvider)
//     {
//       _updateApiClient = updateApiClient;
//       _licenseKeyProvider = licenseKeyProvider;
//       CheckForUpdatesCommand = ReactiveCommand.CreateFromTask(CheckForUpdates);
//       PrepareToUpdateCommand = ReactiveCommand.CreateFromTask(PrepareAsync);
//       LaunchUpdaterCommand = ReactiveCommand.CreateFromTask(LaunchUpdaterAsync);
//
//       CheckForUpdatesCommand.IsExecuting
//         .CombineLatest(PrepareToUpdateCommand.IsExecuting,
//           (checking, downloading) => checking || downloading)
//         .ToPropertyEx(this, _ => _.IsBusy);
//
//       this.WhenAnyValue(_ => _.State)
//         .Select(GetStatusString)
//         .ToPropertyEx(this, _ => _.CurrentStatus);
//
//       var isReadyToUpdate = this.WhenAnyValue(_ => _.State)
//         .Select(state => state == States.ReadyToUpdate);
//
//       isReadyToUpdate
//         .ToPropertyEx(this, _ => _.IsReadyToUpdate);
//
//       this.WhenAnyValue(_ => _.NextVersion)
//         .Select(nextVersion => nextVersion >= AppConstants.CurrentAppVersion)
//         .CombineLatest(isReadyToUpdate, (available, isReady) => available && !isReady)
//         .ToPropertyEx(this, _ => _.CanDownloadUpdates);
//
//       this.WhenAnyValue(_ => _.NextVersion)
//         .Select(nextVersion => nextVersion <= AppConstants.CurrentAppVersion)
//         .ToPropertyEx(this, _ => _.AreCurrentVersionLatest);
//
//       this.WhenAnyValue(_ => _.State)
//         .Select(s => s == States.CheckingForUpdates)
//         .ToPropertyEx(this, _ => _.IsProgressIndeterminate);
//
//       Progress = -1;
//     }
//
//     private string GetStatusString(States state) => state switch
//     {
//       States.CheckingForUpdates => "Checking for Updates...",
//       States.CheckedForUpdates => NextVersion <= AppConstants.CurrentAppVersion
//         ? "No updates are available. You are using latest version."
//         : string.Format("An updated version of {0} is available. Would you like to update to latest version?",
//           AppConstants.ProductFullName),
//       States.DownloadingUpdateInstaller => "Downloading installer...",
//       States.BackingUp => "Creating backup...",
//       States.ReadyToUpdate => "Ready to update...",
//       States.NoUpdatesAvailable => string.Format(
//         "No updates available. You are using latest version of {0}", AppConstants.ProductFullName),
//       _ => string.Empty
//     };
//
//     public string CurrentStatus { [ObservableAsProperty] get; }
//
//     public bool IsBusy { [ObservableAsProperty] get; }
//     public bool IsReadyToUpdate { [ObservableAsProperty] get; }
//     public bool CanDownloadUpdates { [ObservableAsProperty] get; }
//     public bool IsProgressIndeterminate { [ObservableAsProperty] get; }
//     public bool AreCurrentVersionLatest { [ObservableAsProperty] get; }
//     [Reactive] public Version NextVersion { get; set; }
//     [Reactive] public States State { get; private set; }
//     [Reactive] public int Progress { get; set; }
//
//     public ReactiveCommand<Unit, Version> CheckForUpdatesCommand { get; private set; }
//     public ReactiveCommand<Unit, Unit> PrepareToUpdateCommand { get; private set; }
//     public ReactiveCommand<Unit, Unit> LaunchUpdaterCommand { get; private set; }
//
//     private async Task<Version> CheckForUpdates()
//     {
//       State = States.CheckingForUpdates;
//       NextVersion = await _updateApiClient.GetLatestAvailableVersionAsync(_licenseKeyProvider.CurrentLicenseKey);
//       State = States.CheckedForUpdates;
//
//       return NextVersion;
//     }
//
//     private async Task PrepareAsync()
//     {
//       //_installerDirectory = @"C:\Users\alantoo\AppData\Local\Temp\f63a08c9da6d49baaf349de5dacb79a2";
//       //State = States.ReadyToUpdate;
//       //return;
//       State = States.DownloadingUpdateInstaller;
//       _installerDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
//       Directory.CreateDirectory(_installerDirectory);
//       var installerFileName = $"{NextVersion}.exe";
//
//       var installerFullPath = Path.Combine(_installerDirectory, installerFileName);
//       using (var file = File.Create(installerFullPath))
//       {
//         await _updateApiClient.DownloadInstallerAsync(file, NextVersion, Environment.Is64BitOperatingSystem,
//           progressPercents => Progress = progressPercents);
//       }
//
//       Progress = 0;
//       State = States.BackingUp;
//       DirectoryInfo installationDir = new DirectoryInfo(Environment.CurrentDirectory);
//       var backupDirFullPath = Path.Combine(_installerDirectory, "backup");
//       Directory.CreateDirectory(backupDirFullPath);
//
//       var filesToCopy = installationDir.GetFiles("*.*", SearchOption.AllDirectories);
//       var totalFilesSize = (double) filesToCopy.Sum(_ => _.Length);
//
//       long totalCopied = 0L;
//       foreach (FileInfo file in filesToCopy)
//       {
//         var dstFile = file.FullName.Replace(installationDir.FullName, backupDirFullPath);
//         var parentDir = Path.GetDirectoryName(dstFile);
//         if (!Directory.Exists(parentDir))
//         {
//           Directory.CreateDirectory(parentDir);
//         }
//
//         file.CopyTo(dstFile);
//         totalCopied += dstFile.Length;
//         Progress = (int) Math.Floor(totalCopied / totalFilesSize * 100);
//       }
//
//       State = States.ReadyToUpdate;
//     }
//
//     private async Task LaunchUpdaterAsync()
//     {
//       string currentInstallationDir = Environment.CurrentDirectory;
//
//       // Dispatcher.CurrentDispatcher.InvokeAsync(() =>
//       // {
//       var updaterFullPath = Path.Combine(_installerDirectory, "backup", UpdaterExecutableName);
//       var startInfo = new ProcessStartInfo(updaterFullPath,
//         $"\"{_installerDirectory}\" {NextVersion} \"{currentInstallationDir}\"")
//       {
//         UseShellExecute = true
//       };
//
//       Process.Start(startInfo);
//       // });
//     }
//
//     public enum States
//     {
//       CheckingForUpdates,
//       CheckedForUpdates,
//       DownloadingUpdateInstaller,
//       BackingUp,
//       ReadyToUpdate,
//       NoUpdatesAvailable
//     }
//   }
// }