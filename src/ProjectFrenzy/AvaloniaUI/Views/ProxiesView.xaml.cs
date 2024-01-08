using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class ProxiesView : ReactiveUserControl<ProxiesViewModel>
    {
        public ProxiesView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
