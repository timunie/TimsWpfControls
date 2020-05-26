using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace TimsWpfControls.Converter
{
    public class EnumToItemSourceConverter : MarkupExtension, IValueConverter
    {
        static EnumToItemSourceConverter _Instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                return Enum.GetValues(value.GetType());
            }
            throw new ArgumentException("the provided value is not a valid Enum");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _Instance ?? (_Instance = new EnumToItemSourceConverter());
        }
    }
}
