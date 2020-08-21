using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TimsWpfControls.Helper
{
    public static class BindingHelper
    {
        private static readonly DependencyProperty DummyProperty = DependencyProperty.RegisterAttached(
            "Dummy",
            typeof(object),
            typeof(DependencyObject),
            new UIPropertyMetadata(null));

        public static object Eval(object source, string expression)
        {
            Binding binding = new Binding(expression) { Source = source };
            return Eval(binding);
        }

        public static object Eval(object source, string expression, string format)
        {
            Binding binding = new Binding(expression) { Source = source, StringFormat=format };
            return Eval(binding);
        }

        public static object Eval(Binding binding, object source)
        {
            if (binding is null) throw new ArgumentNullException(nameof(binding));

            Binding newBinding = new Binding()
            {
                Source = source,
                AsyncState = binding.AsyncState,
                BindingGroupName = binding.BindingGroupName,
                BindsDirectlyToSource = binding.BindsDirectlyToSource,
                Path = binding.Path,
                Converter = binding.Converter,
                ConverterCulture = binding.ConverterCulture,
                ConverterParameter = binding.ConverterParameter,
                FallbackValue = binding.FallbackValue,
                IsAsync = binding.IsAsync,
                Mode = BindingMode.OneWay,
                StringFormat = binding.StringFormat,
                TargetNullValue = binding.TargetNullValue
            };
            return Eval(newBinding);
        }

        public static object Eval(Binding binding, DependencyObject dependencyObject = null)
        {
            dependencyObject ??= new DependencyObject();
            BindingOperations.SetBinding(dependencyObject, DummyProperty, binding);
            return dependencyObject.GetValue(DummyProperty);
        }
    }
}
