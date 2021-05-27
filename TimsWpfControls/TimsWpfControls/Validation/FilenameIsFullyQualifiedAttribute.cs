using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimsWpfControls.Helper;

namespace TimsWpfControls.Validation
{
    public class FilenameIsFullyQualifiedAttribute : ValidationAttribute
    {
        public FilenameIsFullyQualifiedAttribute() : base(() => Lang.ValidationMessages.FilenameIsNotFullyQualified)
        {

        }

        public override bool IsValid(object value)
        {
            var fileName = value?.ToString();
            return string.IsNullOrWhiteSpace(fileName) || FileHelper.IsFullyQualifiedAdvanced(fileName);
        }
    }
}
