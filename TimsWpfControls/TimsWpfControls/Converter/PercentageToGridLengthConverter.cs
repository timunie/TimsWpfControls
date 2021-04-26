using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TimsWpfControls.Converter
{
    public class PercentageToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            bool inverse = (parameter as string)?.ToLowerInvariant() == "true";

            if (value is double @double)
            {
                if (inverse)
                {
                    @double = 1 - @double;
                }
                return new GridLength(@double, GridUnitType.Star);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
