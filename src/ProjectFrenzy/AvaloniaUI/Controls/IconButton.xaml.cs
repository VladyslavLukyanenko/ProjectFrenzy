using Avalonia;
using Avalonia.Controls;

namespace ProjectFrenzy.AvaloniaUI.Controls
{
    public class IconButton : Button
    {
        public static readonly StyledProperty<string> ActiveIconSrcProperty =
            AvaloniaProperty.Register<IconButton, string>(nameof(ActiveIconSrc));

        public static readonly StyledProperty<string> NormalIconSrcProperty =
            AvaloniaProperty.Register<IconButton, string>(nameof(NormalIconSrc));


        public string ActiveIconSrc
        {
            get => GetValue(ActiveIconSrcProperty);
            set => SetValue(ActiveIconSrcProperty, value);
        }

        public string NormalIconSrc
        {
            get => GetValue(NormalIconSrcProperty);
            set => SetValue(NormalIconSrcProperty, value);
        }
    }
}
