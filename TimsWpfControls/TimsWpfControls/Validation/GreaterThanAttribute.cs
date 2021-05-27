using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimsWpfControls.Validation
{
    public class GreaterThanAttribute : ValidationAttribute
    {
        public object CompareTo { get; private set; }

        public Type DataType { get; private set; }

        public GreaterThanAttribute(double compareTo) : this()
        {
            CompareTo = compareTo;
            DataType = typeof(double);
        }

        public GreaterThanAttribute(int compareTo) : this()
        {
            CompareTo = compareTo;
            DataType = typeof(int);
        }

        private GreaterThanAttribute() : base (() => Lang.ValidationMessages.PropertyMustBeGreaterThanMessage)
        {
            // Empty
        }

        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return true;
            }
            else if (DataType == typeof(double))
            {
                return Convert.ToDouble(value) > (double)CompareTo;
            }
            else if (DataType == typeof(int))
            {
                return Convert.ToInt32(value) > (int)CompareTo;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, this.CompareTo);
        }
    }
}
