using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DicomReader.Avalonia.Converters
{
    public class EnumMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var checkValue = value.ToString();
            var targetValue = parameter.ToString();

            return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var useValue = (bool) value;
            var targetValue = parameter.ToString();

            return useValue ? Enum.Parse(targetType, targetValue) : default;
        }
    }
}
