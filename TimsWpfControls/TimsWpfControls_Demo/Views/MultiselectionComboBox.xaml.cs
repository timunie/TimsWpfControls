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
using TimsWpfControls_Demo.Model;

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

            // demoView.GetAllProperties(multiSelectionComboBox);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // multiSelectionComboBox.SelectedItems.Add(SelectableProperty.Test[0]);
            
            SelectableProperty.Test[0].IsSelected = !SelectableProperty.Test[0].IsSelected;

            // multiSelectionComboBox.SelectedItem = SelectableProperty.Test[0];
        }
    }
}
