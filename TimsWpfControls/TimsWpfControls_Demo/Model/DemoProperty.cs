using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TimsWpfControls.Model;

namespace TimsWpfControls_Demo.Model
{
    public class DemoProperty : BaseClass
    {

		private string _PropertyName;
		public string PropertyName
		{
			get { return _PropertyName; }
			set { _PropertyName = value; RaisePropertyChanged(nameof(PropertyName)); }
		}

		private object _Value;
		public object Value
		{
			get { return _Value; }
			set { _Value = value; RaisePropertyChanged(nameof(Value)); }
		}

		private FrameworkElement _Control;
		public FrameworkElement Control
		{
			get { return _Control; }
			private set { _Control = value; RaisePropertyChanged(nameof(Control)); }
		}

		public void SetControl()
		{
			switch (Value)
			{
				case bool boolVal:
					Control = new CheckBox();
					Control.SetBinding(CheckBox.IsCheckedProperty, "Value");
					break;

				default:
					break;
			}
		}

	}
}
