using Avalonia.Controls;

namespace ProjectFrenzy.AvaloniaUI.Infra.Services.ToastNotifications
{
  public interface INotificationsHostProvider
  {
    StackPanel GetHost();
  }
}