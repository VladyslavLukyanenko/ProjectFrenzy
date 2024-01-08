using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Data.Converters;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.AvaloniaUI.Infra.Converters
{
  public class CheckoutStatusToTextConverter : IValueConverter
  {
    private static readonly IDictionary<CheckoutStatus, string> HumanizedStatusesCache =
      new Dictionary<CheckoutStatus, string>();

    public static readonly CheckoutStatusToTextConverter Instance = new CheckoutStatusToTextConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string result = null;
      if (value is CheckoutStatus s && !HumanizedStatusesCache.TryGetValue(s, out result))
      {
        result = Regex.Replace(s.ToString(), @"([a-z])([A-Z])", "$1 $2");
        HumanizedStatusesCache[s] = result;
      }

      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}