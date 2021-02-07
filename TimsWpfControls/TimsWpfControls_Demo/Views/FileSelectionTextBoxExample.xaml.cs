using System.Windows.Controls;

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
