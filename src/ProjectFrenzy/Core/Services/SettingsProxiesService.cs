using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class SettingsProxiesService : IProxiesService
  {
    private readonly ISettingsService _settingsService;
    private const string SettingsFileName = "Proxies.json";
    private readonly ISourceCache<Proxy, string> _cache = new SourceCache<Proxy, string>(_ => _.Url);
    private readonly List<Proxy> _availableProxies = new List<Proxy>();
    private readonly List<Proxy> _busyProxies = new List<Proxy>();

    public SettingsProxiesService(ISettingsService settingsService)
    {
      _settingsService = settingsService;
      Proxies = _cache.AsObservableCache();
    }

    public IObservableCache<Proxy, string> Proxies { get; }
    public async Task InitializeAsync(CancellationToken ct = default)
    {
      var proxies = await _settingsService.ReadSettingsOrDefaultAsync(SettingsFileName, () => new List<Proxy>(), ct);
      _cache.AddOrUpdate(proxies);
      Reset();
    }

    public async Task CreateAsync(IEnumerable<Proxy> proxies, CancellationToken ct = default)
    {
      _cache.AddOrUpdate(proxies);
      Reset();
      await _settingsService.WriteSettingsAsync(SettingsFileName, _cache.Items, ct);
    }

    public async Task RemoveAsync(Proxy toRemove, CancellationToken ct = default)
    {
      _cache.Remove(toRemove);
      Reset();
      await _settingsService.WriteSettingsAsync(SettingsFileName, _cache.Items, ct);
    }

    private void Reset()
    {
      _availableProxies.Clear();
      _busyProxies.Clear();
      foreach (var proxy in _cache.Items)
      {
        RefreshProxy(proxy);
      }
    }

    private void RefreshProxy(Proxy proxy)
    {
      if (proxy.IsAvailable)
      {
        _availableProxies.Add(proxy);
        _busyProxies.Remove(proxy);
      }
      else
      {
        _busyProxies.Add(proxy);
        _availableProxies.Remove(proxy);
      }
    }

    public Proxy GetUnusedProxy()
    {
      if (_availableProxies.Count == 0)
      {
        return _busyProxies.FirstOrDefault();
      }

      var rnd = new Random((int) DateTime.Now.Ticks);
      var idx = rnd.Next(0, _availableProxies.Count);
      var proxy = _availableProxies[idx];
      proxy.IsAvailable = false;

      RefreshProxy(proxy);
      return proxy;
    }

    public void TryReleaseProxy(Proxy proxy)
    {
      if (proxy == null)
      {
        return;
      }
      
      proxy.IsAvailable = true;
      RefreshProxy(proxy);
    }

    public bool HasAnyProxy => _cache.Items.Any();
  }
}