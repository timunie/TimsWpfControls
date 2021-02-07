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
    /// Interaction logic for FileSelectionTextBoxExample.xaml
    /// </summary>
    public partial class FileSelectionTextBoxExample : UserControl
    {
        public FileSelectionTextBoxExample()
        {
            InitializeComponent();

            ExampleView.GetAllProperties(fileSelectionTextBox);
        }
    }
}
