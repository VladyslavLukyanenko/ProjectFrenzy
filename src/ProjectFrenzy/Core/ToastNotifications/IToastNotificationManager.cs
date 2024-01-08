namespace ProjectFrenzy.Core.ToastNotifications
{
  public interface IToastNotificationManager
  {
    void Clear();
    void Show(ToastContent content);
  }
}