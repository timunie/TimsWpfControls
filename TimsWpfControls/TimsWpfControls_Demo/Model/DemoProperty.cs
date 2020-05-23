using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TimsWpfControls.Model;

namespace TimsWpfControls_Demo.Model
{
    public class DemoProperty : DependencyObject
    {
		public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(DemoProperty), new PropertyMetadata(null));
		public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(DemoProperty), new PropertyMetadata(null));
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DemoProperty), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(object), typeof(DemoProperty), new PropertyMetadata(null));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(object), typeof(DemoProperty), new PropertyMetadata(null));
		public static readonly DependencyProperty ValueTemplateProperty = DependencyProperty.Register("ValueTemplate", typeof(DataTemplate), typeof(DemoProperty), new PropertyMetadata(null));

		public string PropertyName
		{
			get { return (string)GetValue(PropertyNameProperty); }
			set { SetValue(PropertyNameProperty, value); }
		}


		public string GroupName
		{
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}

		public object Value
		{
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		
		public object MinValue
		{
			get { return (object)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}


		public object MaxValue
		{
			get { return (object)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}

		public DataTemplate ValueTemplate
		{
			get { return (DataTemplate)GetValue(ValueTemplateProperty); }
			set { SetValue(ValueTemplateProperty, value); }
		}
	}
}
