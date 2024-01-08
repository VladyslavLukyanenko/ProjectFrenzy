using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public interface IProxiesService
  {
    Task InitializeAsync(CancellationToken ct = default);
    IObservableCache<Proxy, string> Proxies { get; }
    Task CreateAsync(IEnumerable<Proxy> proxies, CancellationToken ct = default);
    Task RemoveAsync(Proxy toRemove, CancellationToken ct = default);
    Proxy GetUnusedProxy();
    void TryReleaseProxy(Proxy proxy);
    bool HasAnyProxy { get; }
  }
}