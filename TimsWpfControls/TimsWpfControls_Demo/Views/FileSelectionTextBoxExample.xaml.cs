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

            ExampleView.AddDemoProperty(FileSelectionTextbox.AcceptsFileDropProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.HeightProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.WidthProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.HorizontalAlignmentProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.VerticalAlignmentProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.ForegroundProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.BackgroundProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.VisibilityProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.BorderThicknessProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.BorderBrushProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.FilterProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.TextProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextbox.IsReadOnlyProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(TextBoxHelper.ClearTextButtonProperty, fileSelectionTextBox);
        }
    }
}
