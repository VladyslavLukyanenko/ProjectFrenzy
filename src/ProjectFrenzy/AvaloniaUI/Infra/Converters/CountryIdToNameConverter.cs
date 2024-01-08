using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using ProjectFrenzy.Core.Services;
using Splat;

namespace ProjectFrenzy.AvaloniaUI.Infra.Converters
{
    public class CountryIdToNameConverter : IValueConverter
    {
        private static readonly IDictionary<string, string> CountryIdToNameCache;
        public static readonly CountryIdToNameConverter Instance = new CountryIdToNameConverter();

        static CountryIdToNameConverter()
        {
            var countriesService = Locator.Current.GetService<ICountriesService>();
            CountryIdToNameCache = countriesService.Countries.ToDictionary(_ => _.Id, _ => _.Title);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string c && CountryIdToNameCache.TryGetValue(c, out var name))
            {
                return name;
            }

            return $"<Invalid Value Provided: {value}>";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}