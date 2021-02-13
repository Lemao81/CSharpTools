﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace DicomReader.WPF.Converters
{
    public class EnumMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }

            var checkValue = value.ToString();
            var targetValue = parameter.ToString();

            return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Binding.DoNothing;
            }

            var useValue = (bool) value;
            var targetValue = parameter.ToString();

            return useValue ? Enum.Parse(targetType, targetValue) : Binding.DoNothing;
        }
    }
}
