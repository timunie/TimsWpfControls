using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Windows;
using TimsWpfControls.Model;

namespace TimsWpfControls_Demo.Model
{
    public class DemoProperty : ObservableObject
    {
        public DemoProperty(DependencyPropertyDescriptor descriptor)
        {
            this.Descriptor = descriptor;
        }

        private DependencyPropertyDescriptor _Descriptor;
        public DependencyPropertyDescriptor Descriptor
        {
            get { return _Descriptor; }
            set { SetProperty(ref _Descriptor, value); }
        }

        private object _Value;
        public object Value
        {
            get { return _Value; }
            set { SetProperty(ref _Value, value); }
        }
    }
}