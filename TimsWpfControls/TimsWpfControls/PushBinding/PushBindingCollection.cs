// see: https://meleak.wordpress.com/2011/08/28/onewaytosource-binding-for-readonly-dependency-property/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace TimsWpfControls
{
    public class PushBindingCollection : FreezableCollection<PushBinding>
    {
        public PushBindingCollection() { }

        public PushBindingCollection(DependencyObject targetObject)
        {
            TargetObject = targetObject;
            ((INotifyCollectionChanged)this).CollectionChanged += CollectionChanged;
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PushBinding pushBinding in e.NewItems)
                {
                    pushBinding.SetupTargetBinding(TargetObject);
                }
            }
        }

        public DependencyObject TargetObject
        {
            get;
            private set;
        }
    }
}
