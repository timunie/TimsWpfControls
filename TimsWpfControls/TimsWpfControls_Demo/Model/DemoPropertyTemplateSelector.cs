using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TimsWpfControls_Demo.Model
{
    public class DemoPropertyTemplateSelector : DataTemplateSelector
    {
        static DemoPropertyTemplateSelector()
        {
            BuildInDataTemplates.Add(typeof(string), (DataTemplate)Application.Current.Resources["Demo.DataTemplates.String"]);
            BuildInDataTemplates.Add(typeof(bool), (DataTemplate)Application.Current.Resources["Demo.DataTemplates.Bool"]);
            BuildInDataTemplates.Add(typeof(bool?), (DataTemplate)Application.Current.Resources["Demo.DataTemplates.Bool.Nullable"]);
            BuildInDataTemplates.Add(typeof(Enum), (DataTemplate)Application.Current.Resources["Demo.DataTemplates.Enum"]);
            BuildInDataTemplates.Add(typeof(double), (DataTemplate)Application.Current.Resources["Demo.DataTemplates.Numeric"]);
            BuildInDataTemplates.Add(typeof(double?), (DataTemplate)Application.Current.Resources["Demo.DataTemplates.Numeric"]);
            BuildInDataTemplates.Add(typeof(int), (DataTemplate)Application.Current.Resources["Demo.DataTemplates.Numeric"]);
            BuildInDataTemplates.Add(typeof(int?), (DataTemplate)Application.Current.Resources["Demo.DataTemplates.Numeric"]);
        }

        public static Dictionary<Type, DataTemplate> BuildInDataTemplates { get; } = new Dictionary<Type, DataTemplate>();


        public DataTemplate FallbackTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DemoProperty demoProperty)
            {
                if (demoProperty.Descriptor.PropertyType.IsEnum)
                {
                    return BuildInDataTemplates[typeof(Enum)];
                }

                return BuildInDataTemplates.TryGetValue(demoProperty.Descriptor.PropertyType, out DataTemplate result) ? result : FallbackTemplate;
            }
            else
            {
                return FallbackTemplate;
            }
        }
    }
}