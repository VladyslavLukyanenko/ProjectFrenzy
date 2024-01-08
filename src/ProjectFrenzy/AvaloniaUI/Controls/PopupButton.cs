using Avalonia;
using Avalonia.Controls;

namespace ProjectFrenzy.AvaloniaUI.Controls
{
  public class PopupButton : ComboBox
  {
    // public static readonly DirectProperty<NotificationToast, ICommand> CommandProperty =
    //   AvaloniaProperty.RegisterDirect<NotificationToast, ICommand>(nameof(Command),
    //     button => button.CloseCommand, (button, command) => button.CloseCommand = command, enableDataValidation: true);
    //
    //
    // private ICommand _command;
    // public ICommand Command
    // {
    //   get => _command;
    //   set => SetAndRaise(CommandProperty, ref _command, value);
    // }
    
    public static readonly StyledProperty<string> TextProperty =
      AvaloniaProperty.Register<NotificationToast, string>(nameof(Text));

    public string Text
    {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
    }
  }
}