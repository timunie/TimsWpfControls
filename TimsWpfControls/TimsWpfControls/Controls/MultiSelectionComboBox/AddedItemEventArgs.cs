﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimsWpfControls
{
    /// <summary>
    /// Provides data for the <see cref="MultiSelectionComboBox.AddedItemEvent"/>
    /// </summary>
    public class AddedItemEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddedItemEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The AddedItemEvent/param>
        /// <param name="source">The source object</param>
        /// <param name="addedItem">The added object</param>
        /// <param name="targetList">The target <see cref="IList"/> where the <see cref="AddedItem"/> should be added</param>
        public AddedItemEventArgs(RoutedEvent routedEvent,
                                   object source,
                                   object addedItem,
                                   IList targetList) : base(routedEvent, source)
        {
            AddedItem = addedItem;
            TargetList = targetList;
        }

        /// <summary>
        /// Gets the added item
        /// </summary>
        public object AddedItem { get; }

        /// <summary>
        /// Gets the <see cref="IList"/> where the <see cref="AddedItem"/> was added to
        /// </summary>
        public IList TargetList { get; }
    }

    /// <summary>
    /// RoutedEventHandler used for the <see cref="MultiSelectionComboBox.AddedItemEvent"/>.
    /// </summary>
    public delegate void AddedItemEventArgsHandler(object sender, AddedItemEventArgs args);
}
