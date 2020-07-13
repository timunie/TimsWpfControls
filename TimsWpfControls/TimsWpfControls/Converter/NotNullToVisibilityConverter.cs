using MahApps.Metro.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimsWpfControls.Converter
{
    public class NotNullToVisibilityConverter : MarkupConverter
    {
        static NotNullToVisibilityConverter instance;

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance ??= new NotNullToVisibilityConverter();
        }
    }
}
