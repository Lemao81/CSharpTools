using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace DicomReader.Avalonia.Converters
{
    public class EnumMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            var checkValue = value.ToString() ?? string.Empty;
            var parameterString = parameter.ToString() ?? string.Empty;
            if (parameterString.Contains(":"))
            {
                var parameters = parameterString.Split(":").Select(p => p.Trim());

                return parameters.Any(p => checkValue.Equals(p, StringComparison.InvariantCultureIgnoreCase));
            }

            return checkValue.Equals(parameterString, StringComparison.InvariantCultureIgnoreCase);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            var useValue = (bool) value;
            var targetValue = parameter.ToString() ?? string.Empty;

            return useValue ? Enum.Parse(targetType, targetValue) : default;
        }
    }
}
