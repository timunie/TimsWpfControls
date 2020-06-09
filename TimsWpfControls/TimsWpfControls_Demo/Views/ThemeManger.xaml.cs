using System.Windows.Controls;
using TimsWpfControls_Demo.Model;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for ThemeManger.xaml
    /// </summary>
    public partial class ThemeManger : UserControl
    {
        MainViewModel viewModel => DataContext as MainViewModel;

        public ThemeManger()
        {
            InitializeComponent();
        }

        private void ColorPicker_DropDownClosed(object sender, System.EventArgs e)
        {
            viewModel.ChangeAppTheme();
        }
    }
}