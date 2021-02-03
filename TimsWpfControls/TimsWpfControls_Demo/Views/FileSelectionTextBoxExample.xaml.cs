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

            ExampleView.AddDemoProperty(FileSelectionTextBox.AllowDropProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.HeightProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.WidthProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.HorizontalAlignmentProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.VerticalAlignmentProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.ForegroundProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.BackgroundProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.VisibilityProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.BorderThicknessProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.BorderBrushProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.FilterStringProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.TextProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(FileSelectionTextBox.IsReadOnlyProperty, fileSelectionTextBox);
            ExampleView.AddDemoProperty(TextBoxHelper.ClearTextButtonProperty, fileSelectionTextBox);
        }
    }
}
