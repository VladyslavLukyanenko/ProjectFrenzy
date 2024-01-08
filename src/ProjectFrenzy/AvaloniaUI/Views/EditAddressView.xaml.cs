using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class EditAddressView
        : ReactiveUserControl<EditAddressViewModel>
    {
        public EditAddressView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
