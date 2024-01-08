using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ProjectFrenzy.AvaloniaUI.Infra.Services.ToastNotifications
{
  public class ActiveWindowNotificationsHostProvider : INotificationsHostProvider
  {
    private const string HostName = "NotificationsHost";

    public StackPanel GetHost()
    {
      return ((IClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime).MainWindow
        .FindControl<StackPanel>(HostName);
    }
  }
}