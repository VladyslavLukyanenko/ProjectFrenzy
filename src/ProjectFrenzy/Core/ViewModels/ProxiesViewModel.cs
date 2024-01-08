using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class ProxiesViewModel
    : ViewModelBase, IRoutableViewModel
  {
    private readonly IProxiesService _proxiesService;
    private readonly IToastNotificationManager _toasts;
    private readonly ReadOnlyObservableCollection<Proxy> _proxies;
    public ProxiesViewModel(IProxiesService proxiesService, IScreen hostScreen, IToastNotificationManager toasts)
    {
      _proxiesService = proxiesService;
      _toasts = toasts;

      HostScreen = hostScreen;
      proxiesService.Proxies.Connect()
        .Bind(out _proxies)
        .DisposeMany()
        .Subscribe();

      proxiesService.Proxies.Connect()
        .CountChanged()
        .Select(_ => _proxies.Count)
        .ToPropertyEx(this, _ => _.Count);

      var canBeCreated = this.WhenAnyValue(_ => _.RawProxies).Select(r => !string.IsNullOrEmpty(r));
      CreateProxiesCommand = ReactiveCommand.CreateFromTask(CreateProxiesAsync, canBeCreated);
      RemoveProxyCommand = ReactiveCommand.CreateFromTask<Proxy>(RemoveProxyAsync);
    }

    private async Task RemoveProxyAsync(Proxy p, CancellationToken ct)
    {
      await _proxiesService.RemoveAsync(p, ct);
      _toasts.Show(ToastContent.Success("Proxy removed."));
    }

    private async Task CreateProxiesAsync(CancellationToken ct)
    {
      if (string.IsNullOrEmpty(RawProxies))
      {
        return;
      }

      var validProxies = new List<Proxy>();
      var invalidProxies = new List<string>();
      foreach (var raw in RawProxies.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
      {
        if (Proxy.TryParse(raw, out var proxy))
        {
          validProxies.Add(proxy);
        }
        else
        {
          invalidProxies.Add(raw);
        }
      }

      await _proxiesService.CreateAsync(validProxies, ct);
      if (invalidProxies.Any())
      {
        _toasts.Show(ToastContent.Error(
          "Some of proxies are malformed. Please check them and try again", "Can't save all proxies"));
        RawProxies = string.Join(Environment.NewLine, invalidProxies);
      }
      else
      {
        _toasts.Show(ToastContent.Success("Proxies are saved."));
        RawProxies = null;
      }
    }

    public ReadOnlyObservableCollection<Proxy> Proxies => _proxies;

    [Reactive] public string RawProxies { get; set; }

    public ReactiveCommand<Unit, Unit> CreateProxiesCommand { get; set; }
    public ReactiveCommand<Proxy, Unit> RemoveProxyCommand { get; set; }

    public int Count { [ObservableAsProperty] get; }
    public string UrlPathSegment => nameof(ProxiesViewModel);
    public IScreen HostScreen { get; }
  }
}