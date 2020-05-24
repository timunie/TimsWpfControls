using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public void AddDemoProperty(DependencyProperty dependencyProperty, DependencyObject bindingTarget, string groupName = null, DataTemplate template = null, object MinValue = null, object MaxValue = null)
        {
            if (dependencyProperty is null) throw new ArgumentNullException(nameof(dependencyProperty));

            var property = new DemoProperty()
            {
                GroupName = groupName ?? dependencyProperty.PropertyType.GetCustomAttribute(typeof(CategoryAttribute))?.ToString(),
                PropertyName = dependencyProperty.Name,
                MinValue = MinValue,
                MaxValue = MaxValue,
                ValueTemplate = template ?? GetBuildInTemplate(dependencyProperty.PropertyType)
            };

            var binding = new Binding(dependencyProperty.Name);
            binding.Source = bindingTarget;
            binding.Mode = BindingMode.TwoWay;

            BindingOperations.SetBinding(property, DemoProperty.ValueProperty, binding);

            DemoProperties.Add(property);
        }

        DataTemplate GetBuildInTemplate (Type type)
        {
            if (type == typeof(Double) || type == typeof(Int32))
            {
               return App.Current.Resources["NumericDataTemplate"] as DataTemplate;
            }
            else if (type == typeof(Boolean))
            {
                return App.Current.Resources["BooleanDataTemplate"] as DataTemplate;
            }

            // Fallback
            return null;
        }
    }
}