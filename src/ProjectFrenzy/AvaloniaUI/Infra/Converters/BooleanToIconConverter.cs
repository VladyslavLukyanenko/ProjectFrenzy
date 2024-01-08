using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProjectFrenzy.AvaloniaUI.Infra.Converters
{
    public class BooleanToIconConverter : IValueConverter
    {
        private const string TrueIcon = "/Assets/Icons/dot_true.png";
        private const string FalseIcon = "/Assets/Icons/dot_false.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BitmapValueConverter.Instance.Convert(value is bool b && b ? TrueIcon : FalseIcon, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
