using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using TimsWpfControls;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for MultiselectionComboBox.xaml
    /// </summary>
    public partial class MultiselectionComboBox : UserControl
    {
        public MultiselectionComboBox()
        {
            InitializeComponent();

            demoView.AddDemoProperty(MultiSelectionComboBox.TextProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(MultiSelectionComboBox.SeparatorProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(TextBoxHelper.ClearTextButtonProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(MultiSelectionComboBox.SelectionModeProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(MultiSelectionComboBox.IsEditableProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(MultiSelectionComboBox.IsReadOnlyProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(MultiSelectionComboBox.TextWrappingProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(MultiSelectionComboBox.AcceptsReturnProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(MultiSelectionComboBox.OrderSelectedItemsByProperty, multiSelectionComboBox);
            demoView.AddDemoProperty(MultiSelectionComboBox.SelectedItemProperty, multiSelectionComboBox);
        }
    }
}
