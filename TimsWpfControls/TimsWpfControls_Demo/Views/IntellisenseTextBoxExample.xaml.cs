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
    /// Interaction logic for IntellisenseTextBoxExample.xaml
    /// </summary>
    public partial class IntellisenseTextBoxExample : UserControl
    {
        private static readonly string xaml = @"
<local:IntellisenseTextBoxExample />
";

        public IntellisenseTextBoxExample()
        {
            InitializeComponent();

            ExampleViewBase.ExampleXaml = xaml;

            ExampleViewBase.GetAllProperties(IntellisenseTextBox);
        }
    }
}
