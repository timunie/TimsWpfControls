using MahApps.Metro.Converters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace TimsWpfControls.Converter
{
    [ValueConversion(typeof(Enum), typeof(bool), ParameterType = typeof(Enum))]
    public class EnumToBoolConverter : MarkupConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? parameter : Binding.DoNothing;
        }
    }
}
