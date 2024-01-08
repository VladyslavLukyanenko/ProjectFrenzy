using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProjectFrenzy.AvaloniaUI.Infra.Converters
{
  public class CheckoutStatusToHighlightClassesConverter : IValueConverter
  {
    public static readonly CheckoutStatusToHighlightClassesConverter Instance =
      new CheckoutStatusToHighlightClassesConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return "SucessText";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}