using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class SettingsEmulatorService : IEmulatorService
  {
    private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly ISettingsService _service;
    private const string SettingsFileName = "Emulators.json";
    private LinkedList<Emulator> _emulators;
    private ISourceCache<Emulator, string> _sourceCache;

    public SettingsEmulatorService(ISettingsService service)
    {
      _service = service;
      Emulators = InitializeCache()
        .AsObservableCache();


      Emulators.Connect().ToObservableChangeSet()
        .Subscribe(async c =>
        {
          await _service.WriteSettingsAsync(SettingsFileName, _sourceCache.Items);
          _emulators = new LinkedList<Emulator>(_sourceCache.Items);
        });
    }

    private IObservable<IChangeSet<Emulator, string>> InitializeCache()
    {
      return ObservableChangeSet.Create<Emulator, string>(async (cache, ct) =>
      {
        _sourceCache = cache;
        var emulators = await _service.ReadSettingsOrDefaultAsync<IList<Emulator>>(SettingsFileName,
          Array.Empty<Emulator>, ct);
        cache.AddOrUpdate(emulators);
      }, _ => _.Ip);
    }

    public IObservableCache<Emulator, string> Emulators { get; }

    public Task RemoveAsync(Emulator toRemove, CancellationToken ct = default)
    {
      _sourceCache.Remove(toRemove);
      return Task.CompletedTask;
    }

    public Task AddOrUpdateAsync(Emulator toAdd, CancellationToken ct = default)
    {
      _sourceCache.AddOrUpdate(toAdd);
      return Task.CompletedTask;
    }

    public Emulator GetAvailableEmulator()
    {
      try
      {
        SemaphoreSlim.Wait();
        return GetAndMoveToEnd();
      }
      finally
      {
        SemaphoreSlim.Release();
      }
    }

    private Emulator GetAndMoveToEnd()
    {
      var emulator = _emulators.FirstOrDefault(_ => _.IsAvailable);
      if (emulator == null)
      {
        return null;
      }

      _emulators.Remove(emulator);
      _emulators.AddLast(emulator);
      return emulator;
    }

    /*public static bool Scan(string ip)
    {
      for (int retries = 0; retries < 5; retries++)
      {
        try
        {

          HttpClient Client = new HttpClient
          {
            Timeout = TimeSpan.FromSeconds(1)
          };

          var Resp = Client.GetAsync(ip + "/ping").Result;
          var Content = Resp.Content.ReadAsStringAsync().Result;
          var JsonResp = JObject.Parse(Program.AndroidEncryption.Decrypt(Content));
          if (JsonResp["version"].ToString() == Versions.server_version)
          {
            Emulator = ip;
            return true;
          }
        }
        catch { }
      }

      return false;
    }*/
  }
}