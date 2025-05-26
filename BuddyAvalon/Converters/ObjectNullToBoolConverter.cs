using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BuddyAvalon.Converters
{
    public class ObjectNullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = value != null;
            return parameter?.ToString()?.ToLower() == "false" ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
