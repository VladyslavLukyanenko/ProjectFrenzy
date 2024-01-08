using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class ProductItemPickerView : UserControl
    {
        public ProductItemPickerView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
