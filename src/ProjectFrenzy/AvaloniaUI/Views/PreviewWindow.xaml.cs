using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class PreviewWindow
        : Window
    {
        public PreviewWindow() : this(null)
        {
        }

        public PreviewWindow(Bitmap renderedControlBitmap)
        {
            RenderedControlBitmap = renderedControlBitmap;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            //this.WhenActivated(d =>
            //{
            //});
        }

        [Reactive] public Bitmap RenderedControlBitmap { get; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
