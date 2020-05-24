using System.Windows;
using System.Windows.Controls;

namespace TimsWpfControls_Demo.Model
{
    public class DemoPropertyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FallbackTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DemoProperty demoProperty)
            {
                return demoProperty.ValueTemplate ?? FallbackTemplate;
            }
            else
            {
                return FallbackTemplate;
            }
        }
    }
}