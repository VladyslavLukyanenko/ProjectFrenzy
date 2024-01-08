using Avalonia;
using Avalonia.Markup.Xaml;
using ProjectFrenzy.AvaloniaUI.Controls;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class LoginView : MetroWindow
    {
        public LoginView()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public LoginViewModel ViewModel { get; set; }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
