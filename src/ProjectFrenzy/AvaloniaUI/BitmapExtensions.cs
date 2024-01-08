using System;
using Avalonia.Visuals.Media.Imaging;
using SkiaSharp;

namespace ProjectFrenzy.AvaloniaUI
{
  public static class BitmapExtensions
  {
    // public static Bitmap CreateScaledBitmap(this Bitmap self, PixelSize destinationSize,
    //   BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
    // {
    //   var stream = new MemoryStream();
    //   self.Save(stream);
    //   stream.Position = 0;
    //   SKCodec codec = SKCodec.Create(stream);
    //   SKImageInfo info = codec.Info;
    //
    //   SKSizeI supportedScale = codec.GetScaledDimensions((float) destinationSize.Width / info.Width);
    //
    //   SKImageInfo nearest = new SKImageInfo(supportedScale.Width, supportedScale.Height);
    //   SKBitmap bmp = SKBitmap.Decode(codec, nearest);
    //
    //   SKImageInfo desired = new SKImageInfo(destinationSize.Width, destinationSize.Height, SKColorType.Bgra8888);
    //   bmp = bmp.Resize(desired, interpolationMode.ToSKFilterQuality());
    //
    //   SKImage image = SKImage.FromBitmap(bmp);
    //
    //
    //   var picture = new Bitmap(image.Encode().AsStream());
    //   self.Dispose();
    //   return picture;
    // }
    
    public static SKFilterQuality ToSKFilterQuality(this BitmapInterpolationMode interpolationMode)
    {
      switch (interpolationMode)
      {
        case BitmapInterpolationMode.LowQuality:
          return SKFilterQuality.Low;
        case BitmapInterpolationMode.MediumQuality:
          return SKFilterQuality.Medium;
        case BitmapInterpolationMode.HighQuality:
          return SKFilterQuality.High;
        case BitmapInterpolationMode.Default:
          return SKFilterQuality.None;
        default:
          throw new ArgumentOutOfRangeException(nameof(interpolationMode), interpolationMode, null);
      }
    }
  }
}