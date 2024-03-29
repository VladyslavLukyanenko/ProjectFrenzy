﻿using System;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace ProjectFrenzy.AvaloniaUI.Infra.Converters
{
    public class IconImageConverter : IValueConverter
    {
        private static IconImageConverter s_converter = new IconImageConverter();

        public static IconImageConverter Converter => s_converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WindowIcon)
            {
                Bitmap result = null;

                using (var stream = new MemoryStream())
                {
                    (value as WindowIcon).Save(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    result = new Bitmap(stream);
                }

                return result;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}