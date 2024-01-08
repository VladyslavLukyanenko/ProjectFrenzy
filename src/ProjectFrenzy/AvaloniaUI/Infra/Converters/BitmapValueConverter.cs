using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Visuals.Media.Imaging;
using SkiaSharp;

namespace ProjectFrenzy.AvaloniaUI.Infra.Converters
{
  public class BitmapValueConverter : IValueConverter
  {
    private static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
    public static readonly BitmapValueConverter Instance = new BitmapValueConverter();
    private static HttpClient HttpClient = new HttpClient();
    private static IDictionary<string, Bitmap> PicturesCache = new ConcurrentDictionary<string, Bitmap>();
    private static SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

    private static readonly IDictionary<string, SemaphoreSlim> SemaphoreSlims = new Dictionary<string, SemaphoreSlim>();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Bitmap bitmap = null;
      if (value is string path && targetType == typeof(IBitmap))
      {
        if (!path.StartsWith("http") && !path.StartsWith("avares"))
        {
          path = $"avares://{AssemblyName}{path}";
        }

        SemaphoreSlim pathSemaphore = null;
        try
        {
          var uri = new Uri(path, UriKind.RelativeOrAbsolute);
          try
          {
            Semaphore.Wait();
            if (!SemaphoreSlims.TryGetValue(path, out pathSemaphore))
            {
              pathSemaphore = new SemaphoreSlim(1, 1);
              SemaphoreSlims[path] = pathSemaphore;
            }

            pathSemaphore.Wait();
          }
          finally
          {
            Semaphore.Release();
          }
          
          
          if (!PicturesCache.TryGetValue(path, out bitmap))
          {
            bitmap = uri.Scheme switch
            {
              "file" => new Bitmap((string) value),
              "http" => DownloadPicture(uri),
              "https" => DownloadPicture(uri),
              _ => ReadFromAssets(uri)
            };

            PicturesCache[path] = bitmap;
          }
        }
        finally
        {
          pathSemaphore?.Release();
        }
      }

      return bitmap;
    }

    private static Bitmap ReadFromAssets(Uri uri)
    {
      var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
      return new Bitmap(assets.Open(uri));
    }

    private static int MaxWidth = 150;

    private static Bitmap DownloadPicture(Uri uri)
    {
      var r = HttpClient.GetAsync(uri).GetAwaiter().GetResult();
      var pictureStream = r.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
      
      SKCodec codec = SKCodec.Create(pictureStream);
      SKImageInfo info = codec.Info;
      if (info.Width < MaxWidth)
      {
        pictureStream.Position = 0;
        return new Bitmap(pictureStream);
      }

      var scale = MaxWidth / (double) info.Width;
      var destinationSize = new PixelSize((int) (info.Width * scale), (int) (info.Height * scale));
      SKSizeI supportedScale = codec.GetScaledDimensions((float) destinationSize.Width / info.Width);

      SKImageInfo nearest = new SKImageInfo(supportedScale.Width, supportedScale.Height);
      SKBitmap bmp = SKBitmap.Decode(codec, nearest);

      SKImageInfo desired = new SKImageInfo(destinationSize.Width, destinationSize.Height, SKColorType.Bgra8888);
      bmp = bmp.Resize(desired, BitmapInterpolationMode.HighQuality.ToSKFilterQuality());

      SKImage image = SKImage.FromBitmap(bmp);

      var picture = new Bitmap(image.Encode().AsStream());
      return picture;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}