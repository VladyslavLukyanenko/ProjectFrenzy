using System;
using System.Globalization;
using Avalonia.Data.Converters;
using ProjectFrenzy.Core;

namespace ProjectFrenzy.AvaloniaUI.Infra.Converters
{
  public class HumanReadableTimeToStringConverter : IValueConverter
  {
    public static readonly HumanReadableTimeToStringConverter Instance = new HumanReadableTimeToStringConverter();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((TimeSpan) value).ToHumanReadableString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}