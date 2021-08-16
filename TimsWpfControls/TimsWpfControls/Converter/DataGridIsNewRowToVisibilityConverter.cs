using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TimsWpfControls
{
    public class DataGridIsNewRowToVisibilityConverter : IValueConverter
    {
        private static DataGridIsNewRowToVisibilityConverter _Instance;
        public static DataGridIsNewRowToVisibilityConverter Instance => _Instance ??= new();

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == CollectionView.NewItemPlaceholder ? Visibility.Collapsed : Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
