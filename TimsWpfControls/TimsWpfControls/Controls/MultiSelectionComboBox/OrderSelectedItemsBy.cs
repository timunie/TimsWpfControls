using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimsWpfControls
{
    /// <summary>
    /// Defines how the selected Items should be arranged for display
    /// </summary>
    public enum OrderSelectedItemsBy
    {
        /// <summary>
        /// Displays the selected items in the same order as they were selected
        /// </summary>
        SelectedOrder,

        /// <summary>
        /// Displays the selected items in the same order as they are stored in the ItemsSource
        /// </summary>
        ItemsSourceOrder
    }
}
