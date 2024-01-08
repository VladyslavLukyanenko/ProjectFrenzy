using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Threading;
using ProjectFrenzy.AvaloniaUI.Controls;
using ProjectFrenzy.Core.ToastNotifications;
using ReactiveUI;

namespace ProjectFrenzy.AvaloniaUI.Infra.Services.ToastNotifications
{
  public class AvaloniaToastNotificationManager : IToastNotificationManager
  {
    private const int VisibleNotificationsLimit = 7;
    private readonly INotificationsHostProvider _notificationsHostProvider;
    private readonly LinkedList<NotificationToast> _spawnedToasts = new LinkedList<NotificationToast>();

    public AvaloniaToastNotificationManager(INotificationsHostProvider notificationsHostProvider)
    {
      _notificationsHostProvider = notificationsHostProvider;
    }

    public void Clear()
    {
      var host = _notificationsHostProvider.GetHost();
      host.Children.Clear();
    }

    public void Show(ToastContent content)
    {
      Dispatcher.UIThread.InvokeAsync(() =>
      {
        var toast = new NotificationToast
        {
          Title = content.Title,
          MessageContent = content.Content,
          Type = content.Type,
          LifetimeDuration = content.AutoCloseTimeout
        };

        EnsureCanAddOneMoreToast();
        _spawnedToasts.AddLast(toast);

        var host = _notificationsHostProvider.GetHost();
        toast.CloseCommand = ReactiveCommand.Create(() =>
        {
          host.Children.Remove(toast);
          _spawnedToasts.Remove(toast);
        });

        toast.Command = toast.CloseCommand;

        Observable.Return(toast)
          .Take(1)
          .Delay(content.AutoCloseTimeout)
          .ObserveOn(RxApp.MainThreadScheduler)
          .Subscribe(t => { t.CloseCommand.Execute(null); });

        host.Children.Add(toast);
      });
    }

    private void EnsureCanAddOneMoreToast()
    {
      if (!_spawnedToasts.Any() || _spawnedToasts.Count + 1 < VisibleNotificationsLimit)
      {
        return;
      }

      _spawnedToasts.First?.Value?.CloseCommand.Execute(null);
      ;
    }
  }
}