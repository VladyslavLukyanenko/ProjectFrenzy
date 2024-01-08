using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class FlashsaleItemPickerView : UserControl
    {
        public FlashsaleItemPickerView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
