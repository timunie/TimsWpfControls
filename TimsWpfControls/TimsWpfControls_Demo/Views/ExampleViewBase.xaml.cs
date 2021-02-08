using ICSharpCode.AvalonEdit;
using Microsoft.Xaml.Behaviors;
using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TimsWpfControls_Demo.Model;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for ExampleViewBase.xaml
    /// </summary>
    /// 
    [TemplatePart(Name = nameof(PART_XamlTextEditor), Type = typeof(TextEditor))]
    public partial class ExampleViewBase : ContentControl
    {
        public ExampleViewBase()
        {
            // DemoProperties.CollectionChanged += DemoProperties_CollectionChanged;
        }

        private void DemoProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                FillExampleXaml();
            }
        }

        private TextEditor PART_XamlTextEditor;
        public ObservableCollection<DemoProperty> DemoProperties { get; } = new ObservableCollection<DemoProperty>();



        public static readonly DependencyProperty ExampleXamlProperty = DependencyProperty.Register("ExampleXaml", typeof(string), typeof(ExampleViewBase), new PropertyMetadata(null, ExampleXamlChanged));



        public string ExampleXaml
        {
            get { return (string)GetValue(ExampleXamlProperty); }
            set { SetValue(ExampleXamlProperty, value); }
        }

        private static void ExampleXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExampleViewBase exampleView && exampleView.PART_XamlTextEditor != null)
            {
                exampleView.PART_XamlTextEditor.Text = exampleView.FillExampleXaml();
            }
        }

        private string FillExampleXaml ()
        {
            var result = ExampleXaml;

            if (string.IsNullOrEmpty(result)) return null;

            foreach (var property in DemoProperties)
            {
               result = result.Replace($"[{property.Descriptor.Name}]", property.Value?.ToString());
            }

            return result;
        }

        public void AddDemoProperty(DependencyPropertyDescriptor descriptor, FrameworkElement bindingTarget)
        {
            if (descriptor.IsReadOnly || descriptor.DesignTimeOnly || !descriptor.IsBrowsable || descriptor.IsAttached) return;

            var demoProperty = new DemoProperty(descriptor);

            DemoProperties.Add(demoProperty);

            try
            {
                demoProperty.Value = bindingTarget.GetValue(descriptor.DependencyProperty);
               
                var binding = new Binding(nameof(DemoProperty.Value))
                {
                    Source = demoProperty,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };

                bindingTarget.SetBinding(descriptor.DependencyProperty, binding);
            }
            catch (Exception e)
            {

            }
        }


        public void GetAllProperties(FrameworkElement dependencyObject)
        {
            foreach (var property in TypeDescriptor.GetProperties(dependencyObject))
            {
                if (DependencyPropertyDescriptor.FromProperty(property as PropertyDescriptor) is DependencyPropertyDescriptor descriptor)
                {
                    AddDemoProperty(descriptor, dependencyObject);
                }
            }
        }


        //DataTemplate GetBuildInTemplate (Type type)
        //{
        //    if (type == typeof(object))
        //    {
        //        return App.Current.Resources["StringDataTemplate"] as DataTemplate;
        //    }
        //    else if (type == typeof(double) || 
        //        type == typeof(int) ||
        //        type == typeof(float) ||
        //        type == typeof(byte) ||
        //        type == typeof(double?) ||
        //        type == typeof(int?) ||
        //        type == typeof(float?) ||
        //        type == typeof(byte?))
        //    {
        //       return App.Current.Resources["NumericDataTemplate"] as DataTemplate;
        //    }
        //    else if (type == typeof(bool) || type == typeof(bool?))
        //    {
        //        return App.Current.Resources["BooleanDataTemplate"] as DataTemplate;
        //    }
        //    else if(type.IsAssignableFrom(typeof(Brush)))
        //    {
        //        return App.Current.Resources["BrushDataTemplate"] as DataTemplate;
        //    }
        //    else if (type.IsAssignableFrom(typeof(string)))
        //    {
        //        return App.Current.Resources["StringDataTemplate"] as DataTemplate;
        //    }
        //    else if (type.IsEnum)
        //    {
        //        return App.Current.Resources["EnumDataTemplate"] as DataTemplate;
        //    }

        //    // Fallback
        //    return App.Current.Resources["StringDataTemplate"] as DataTemplate; ;
        //}


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_XamlTextEditor = this.GetTemplateChild(nameof(PART_XamlTextEditor)) as TextEditor;
            PART_XamlTextEditor.Text = FillExampleXaml();
        }
    }
}