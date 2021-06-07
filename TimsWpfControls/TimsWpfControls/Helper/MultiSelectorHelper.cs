using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TimsWpfControls
{
    /// <summary>
    /// Defines a helper class for selected items binding on collections with multiselector elements
    /// </summary>
    public static class MultiSelectorHelper
    {
        public static readonly DependencyProperty SelectedItemsProperty
            = DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(MultiSelectorHelper),
                new FrameworkPropertyMetadata(null, OnSelectedItemsChanged));

        /// <summary>
        /// Handles disposal and creation of old and new bindings
        /// </summary>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ListBox || d is MultiSelector || d is MultiSelectionComboBox)) throw new ArgumentException("The property 'SelectedItems' may only be set on ListBox, MultiSelector or MultiSelectionComboBox elements.");

            if (e.OldValue != e.NewValue)
            {
                var oldBinding = GetSelectedItemBinding(d);
                oldBinding?.UnBind();

                if (e.NewValue != null)
                {
                    var multiSelectorBinding = new MultiSelectorBinding((Selector)d, (IList)e.NewValue);
                    SetSelectedItemBinding(d, multiSelectorBinding);
                    multiSelectorBinding.Bind();
                }
            }
        }

        /// <summary>
        /// Gets the selected items property binding
        /// </summary>
        [Category("TimsWpfControls")]
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        public static IList GetSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        /// <summary>
        /// Sets the selected items property binding
        /// </summary>
        [Category("TimsWpfControls")]
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        public static void SetSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        private static readonly DependencyProperty SelectedItemBindingProperty
            = DependencyProperty.RegisterAttached(
                "SelectedItemBinding",
                typeof(MultiSelectorBinding),
                typeof(MultiSelectorHelper));

        /// <summary>
        /// Gets the <see cref="MultiSelectorBinding"/> for a binding
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        private static MultiSelectorBinding GetSelectedItemBinding(DependencyObject element)
        {
            return (MultiSelectorBinding)element.GetValue(SelectedItemBindingProperty);
        }

        /// <summary>
        /// Sets the <see cref="MultiSelectorBinding"/> for a bining
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ListBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelector))]
        private static void SetSelectedItemBinding(DependencyObject element, MultiSelectorBinding value)
        {
            element.SetValue(SelectedItemBindingProperty, value);
        }

        /// <summary>
        /// Defines a binding between multi selector and property
        /// </summary>
        private class MultiSelectorBinding
        {
            private readonly IList _collection;
            private readonly ObservableCollection<object> _selectedItems;

            /// <summary>
            /// Creates an instance of <see cref="MultiSelectorBinding"/>
            /// </summary>
            /// <param name="selector">The selector of this binding</param>
            /// <param name="collection">The bound collection</param>
            public MultiSelectorBinding(Selector selector, IList collection)
            {
                _collection = collection;

                if (selector is ListBox listbox)
                {
                    _selectedItems = listbox.SelectedItems as ObservableCollection<object>;
                }
                else if (selector is MultiSelector multiSelector)
                {
                    _selectedItems = multiSelector.SelectedItems as ObservableCollection<object>;
                }
                else if (selector is MultiSelectionComboBox multiSelectionComboBox)
                {
                    _selectedItems = multiSelectionComboBox.SelectedItems as ObservableCollection<object>;
                }

                _selectedItems.Clear();
                foreach (var newItem in collection)
                {
                    _selectedItems.Add(newItem);
                }
            }

            /// <summary>
            /// Registers the event handlers for selector and collection changes
            /// </summary>
            public void Bind()
            {
                // prevent multiple event registration
                UnBind();

                _selectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;

                if (_collection is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged += this.OnCollectionChanged;
                }
            }


            /// <summary>
            /// Unregisters the event handlers for selector and collection changes
            /// </summary>
            public void UnBind()
            {
                _selectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                if (_collection is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged -= this.OnCollectionChanged;
                }
            }


            /// <summary>
            /// Updates the selector with changes made in the collection
            /// </summary>
            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                _selectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;

                try
                {
                    SyncCollections(_collection, _selectedItems, e);
                }
                finally
                {
                    _selectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
                }
            }

            /// <summary>
            /// Updates the source collection with changes made in the selector
            /// </summary>
            private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (_collection is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)_collection).CollectionChanged -= OnCollectionChanged;
                }

                try
                {
                    SyncCollections(_selectedItems, _collection, e);
                }
                finally
                {
                    if (_collection is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)_collection).CollectionChanged += OnCollectionChanged;
                    }
                }
            }

            internal static void SyncCollections(IList sourceCollection, IList targetCollection, NotifyCollectionChangedEventArgs e)
            {
                if (sourceCollection is null || targetCollection is null)
                {
                    return;
                }


                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems)
                        {
                            targetCollection.Add(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in e.OldItems)
                        {
                            targetCollection.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        foreach (var item in e.NewItems)
                        {
                            targetCollection.Add(item);
                        }
                        foreach (var item in e.OldItems)
                        {
                            targetCollection.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        targetCollection.Clear();

                        for (int i = 0; i < sourceCollection.Count; i++)
                        {
                            targetCollection.Add(sourceCollection[i]);
                        }
                        break;
                }
            }
        }
    }
}
