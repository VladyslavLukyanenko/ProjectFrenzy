﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProjectFrenzy.AvaloniaUI.Infra.Converters
{
    public class EnumExpectedValueToBooleanConverter : IValueConverter
    {
        public static readonly EnumExpectedValueToBooleanConverter Instance = new EnumExpectedValueToBooleanConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}