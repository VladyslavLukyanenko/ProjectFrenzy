using Avalonia.Media.Imaging;

namespace ProjectFrenzy.AvaloniaUI.Infra.Services
{
    public interface IWindowContentPictureProvider
    {
        Bitmap GetBluredBackground();
    }
}