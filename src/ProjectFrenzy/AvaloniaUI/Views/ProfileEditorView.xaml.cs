using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class ProfileEditorView
        : ReactiveUserControl<ProfileEditorViewModel>
    {
        public ProfileEditorView()
        {
            this.InitializeComponent();

            //this.WhenActivated(d =>
            //{
            //    var provider = Locator.Current.GetService<IWindowContentPictureProvider>();
            //    Background = new ImageBrush(provider.GetBluredBackground());
            //});
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
