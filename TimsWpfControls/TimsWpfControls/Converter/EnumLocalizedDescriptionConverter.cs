using MahApps.Metro.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimsWpfControls.Model;

namespace TimsWpfControls.Converter
{
    public class EnumLocalizedDescriptionConverter : MarkupConverter
    {
        EnumLocalizedDescriptionConverter _Instance;
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Enum inputEnum))
            {
                return null;
            }

            if (inputEnum.GetType().GetField(inputEnum.ToString()).GetCustomAttributes(false).OfType<LocalizedDescriptionAttribute>().FirstOrDefault() is LocalizedDescriptionAttribute descriptionAttribute)
            {
                return descriptionAttribute.Description;
            }
            else
            {
                return inputEnum.ToString();
            }
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _Instance ??= new EnumLocalizedDescriptionConverter();
        }
    }
}
