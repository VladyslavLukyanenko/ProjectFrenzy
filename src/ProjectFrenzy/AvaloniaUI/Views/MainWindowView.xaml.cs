using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering;
using ProjectFrenzy.AvaloniaUI.Controls;
using ProjectFrenzy.AvaloniaUI.Infra.Services;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI;
using SkiaSharp;

namespace ProjectFrenzy.AvaloniaUI.Views
{
    public class MainWindowView
        : MetroWindow, IWindowContentPictureProvider
    {
        public MainWindowView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            
            //this.WhenActivated(d =>
            //{
            //});
        }

        public MainWindowViewModel ViewModel { get; set; }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            var stackPanel = this.FindControl<StackPanel>("NavContainer");
            stackPanel.Children.AddRange(ViewModel.NavigationButtons.Select(b =>
            {
                var btn = new NavButton
                {
                    Command = b.Command,
                    ActiveIconSrc = b.ActiveIconSrc,
                    NormalIconSrc = b.NormalIconSrc,
                    [!NavButton.IsActiveProperty] = b.WhenAnyValue(_ => _.IsActivated).ToBinding()
                };

                return btn;
            }));

            ViewModel.NavigationButtons.First().Command.Execute().Subscribe();
        }

        public Bitmap GetBluredBackground()
        {
            var target = this.FindControl<Grid>("ContentContainer");
            
            var mem = RenderToStream(target, target.Bounds.Size);
            return new Bitmap(mem);
            // var preview = new PreviewWindow(bitmap);
            // preview.ShowDialog(this);
        }
        public void ShowBlured()
        {
            var preview = new PreviewWindow(GetBluredBackground());
            preview.ShowDialog(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private Stream RenderToStream(Control target, Size size)
        {
            double dpi = 96;
            var factory = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
            var pixelSize = new PixelSize((int) size.Width, (int) size.Height);
            var dpiVector = new Vector(dpi, dpi);

            var mem = new MemoryStream();
            using (var rtb = factory.CreateRenderTargetBitmap(pixelSize, dpiVector))
            using (var renderer = new DeferredRenderer(target, rtb))
            {
                target.Measure(size);
                target.Arrange(new Rect(size));
                ((IRenderLoopTask) renderer).Update(TimeSpan.Zero);
                ((IRenderLoopTask) renderer).Render();

                rtb.Save(mem);
            }

            mem.Position = 0;
            return AddBlur(mem);
        }

        Stream AddBlur(Stream picture)
        {
            SKBitmap bitmap = SKBitmap.Decode(picture);
            SKImageInfo info = bitmap.Info;
            SKCanvas canvas = new SKCanvas(bitmap);

            // Get values from sliders
            float sigmaX = 4;
            float sigmaY = 3;
            // canvas.Clear(SKColors.Black);
            using (SKPaint paint = new SKPaint())
            {
                paint.ImageFilter = SKImageFilter.CreateBlur(sigmaX, sigmaY);
                SKRect bitmapRect = new SKRect(0, 0, info.Width, info.Height);
                canvas.DrawBitmap(bitmap, bitmapRect, paint: paint);
            }

            var image = SKImage.FromBitmap(bitmap);
            return new MemoryStream(image.Encode().ToArray());
        }

        public IObservable<Bitmap> BluredContent { get; }
    }
}