using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimsWpfControls_Demo.Model;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for ExampleView_Base.xaml
    /// </summary>
    public partial class ExampleView_Base : ContentControl
    {
        public static readonly DependencyProperty DemoPropertiesProperty = DependencyProperty.Register("DemoProperties", typeof(ObservableCollection<DemoProperty>), typeof(ExampleView_Base), new PropertyMetadata(null));
        
        
        public ExampleView_Base()
        {
            DemoProperties = new ObservableCollection<DemoProperty>();
        }

        public ObservableCollection<DemoProperty> DemoProperties
        {
            get { return (ObservableCollection<DemoProperty>)GetValue(DemoPropertiesProperty); }
            set { SetValue(DemoPropertiesProperty, value); }
        }
    }
}
