using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class EmulatorsAvailabilityWatcher : IEmulatorsAvailabilityWatcher
  {
    private readonly IEmulatorService _emulatorService;
    private ReadOnlyObservableCollection<Emulator> _emulators;

    public EmulatorsAvailabilityWatcher(IEmulatorService emulatorService)
    {
      _emulatorService = emulatorService;
    }


    public void Spawn()
    {
      WriteLine("Spawning emulator availability watcher");
      _emulatorService.Emulators.Connect()
        .Bind(out _emulators)
        .Subscribe();

      Observable.Interval(TimeSpan.FromSeconds(30))
        .Where(_ => _emulators.Any())
        .Subscribe(_ => RefreshAvailability());
      
      RefreshAvailability();
      WriteLine("Spawned emulator availability watcher");
      
      void RefreshAvailability() => Task.WhenAll(_emulators.Select(p => UpdateAvailabilityAsync(p)));
    }

    public async Task UpdateAvailabilityAsync(Emulator emulator, CancellationToken ct = default)
    {
      emulator.IsAvailable = await PingAsync(emulator, ct);
      WriteLine($"Emulator {emulator.Ip} IsAvailable={emulator.IsAvailable}");
    }

    public async Task<bool> PingAsync(Emulator emulator, CancellationToken ct = default)
    {
      var http = new HttpClient
      {
        Timeout = TimeSpan.FromSeconds(5)
      };

      try
      {
        var r = await http.GetAsync(emulator.GetUrl() + "/ping", HttpCompletionOption.ResponseHeadersRead, ct);
        return r.IsSuccessStatusCode;
      }
      catch
      {
        return false;
      }
    }

    private static void WriteLine(string message)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }
  }
}