using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimsWpfControls.Validation
{
    public class CollectionNotEmptyAttribute : ValidationAttribute
    {
        public CollectionNotEmptyAttribute() : base(() => Lang.ValidationMessages.CollectionMayNotBeEmpty)
        {

        }

        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return true;
            }
            if (value is IEnumerable collection)
            {
                return collection.GetEnumerator().MoveNext();
            }
            else
            {
                throw new ArgumentException(null, nameof(value));
            }
        }
    }
}
