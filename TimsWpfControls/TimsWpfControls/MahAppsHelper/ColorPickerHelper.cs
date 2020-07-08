using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TimsWpfControls.Model;

namespace TimsWpfControls
{
    public static class ColorPickerHelper
    {
        static RelayCommand _ClearColorCommand;
        public static RelayCommand ClearColorCommand
        {
            get
            {
                return _ClearColorCommand ??= new RelayCommand((param) =>
                {
                    if (param is ColorPickerBase colorPicker)
                    {
                        colorPicker.SetCurrentValue(ColorPickerBase.SelectedColorProperty, null);
                    }
                });
            }
        }



        public static bool GetClearColorButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClearColorButtonProperty);
        }

        public static void SetClearColorButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearColorButtonProperty, value);
        }

        public static readonly DependencyProperty ClearColorButtonProperty = DependencyProperty.RegisterAttached("ClearColorButton", typeof(bool), typeof(ColorPickerHelper), new PropertyMetadata(false));


    }
}
