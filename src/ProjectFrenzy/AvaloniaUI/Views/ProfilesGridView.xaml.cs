using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class ProfilesGridView : ReactiveUserControl<ProfilesGridViewModel>
    {
        public ProfilesGridView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
