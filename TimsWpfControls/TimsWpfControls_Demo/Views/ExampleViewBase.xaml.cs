using ICSharpCode.AvalonEdit;
using Microsoft.Xaml.Behaviors;
using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TimsWpfControls_Demo.Model;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for ExampleViewBase.xaml
    /// </summary>
    /// 
    [TemplatePart(Name = nameof(PART_XamlTextEditor), Type = typeof(TextEditor))]
    [TemplatePart(Name = nameof(PART_PropertiesList), Type = typeof(DataGrid))]
    public partial class ExampleViewBase : ContentControl
    {


        public ExampleViewBase()
        {
            DemoProperties.CollectionChanged += DemoProperties_CollectionChanged;
        }

        private void DemoProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                foreach (DemoProperty demoProperty in e.NewItems)
                {
                    demoProperty.PropertyChanged += DemoProperty_PropertyChanged;
                }

                FillExampleXaml();
            }

            if (e.OldItems is not null)
            {
                foreach (DemoProperty demoProperty in e.OldItems)
                {
                    demoProperty.PropertyChanged += DemoProperty_PropertyChanged;
                }

                FillExampleXaml();
            }
        }

        private void DemoProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FillExampleXaml();
        }

        private TextEditor PART_XamlTextEditor;
        private DataGrid PART_PropertiesList;

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
                exampleView.FillExampleXaml();
            }
        }

        bool canFillXaml = true;
        internal void FillExampleXaml ()
        {
            if (!canFillXaml || PART_XamlTextEditor is null) return;

            var result = ExampleXaml;

            if (string.IsNullOrEmpty(result) && ExampleContent is not null)
            {
                // result = XamlWriter.Save(ExampleContent);
            }
            else
            {
                foreach (var property in DemoProperties)
                {
                    result = result.Replace($"[{property.Descriptor.Name}]", property.Value?.ToString());
                }
            }
            
            PART_XamlTextEditor.Text = result;
        }


        /// <summary>Identifies the <see cref="ExampleContent"/> dependency property.</summary>
        public static readonly DependencyProperty ExampleContentProperty =
            DependencyProperty.Register(nameof(ExampleContent), typeof(FrameworkElement), typeof(ExampleViewBase), new PropertyMetadata(null));

        public object ExampleContent
        {
            get { return (object)GetValue(ExampleContentProperty); }
            set { SetValue(ExampleContentProperty, value); }
        }

        public void AddDemoProperty(DependencyPropertyDescriptor descriptor, FrameworkElement bindingTarget)
        {
            if (descriptor.IsReadOnly || descriptor.DesignTimeOnly || !descriptor.IsBrowsable) return;

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
                Debug.WriteLine($"{e.Message} ({e.Source}");
            }
        }


        public void GetAllProperties(FrameworkElement dependencyObject)
        {
            ExampleContent = dependencyObject;

            canFillXaml = false;

            foreach (var property in TypeDescriptor.GetProperties(dependencyObject))
            {
                if (DependencyPropertyDescriptor.FromProperty(property as PropertyDescriptor) is DependencyPropertyDescriptor descriptor)
                {
                    AddDemoProperty(descriptor, dependencyObject);
                }
            }

            canFillXaml = true;
            FillExampleXaml();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_XamlTextEditor = this.GetTemplateChild(nameof(PART_XamlTextEditor)) as TextEditor;
            FillExampleXaml();

            if (PART_PropertiesList is not null) PART_PropertiesList.SizeChanged -= PART_PropertiesList_SizeChanged;
            PART_PropertiesList = this.GetTemplateChild(nameof(PART_PropertiesList)) as DataGrid;
            PART_PropertiesList.SizeChanged += PART_PropertiesList_SizeChanged;

        }

        private void PART_PropertiesList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                foreach (var column in dataGrid.Columns)
                {
                    var size = column.Width;
                    column.ClearValue(DataGrid.ColumnWidthProperty);
                    column.SetValue(DataGrid.ColumnWidthProperty, size);
                }
            }
        }
    }
}