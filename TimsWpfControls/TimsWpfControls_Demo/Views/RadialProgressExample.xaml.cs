using System.Windows.Controls;
using TimsWpfControls;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for RadialProgressExample.xaml
    /// </summary>
    public partial class RadialProgressExample : UserControl
    {
        public RadialProgressExample()
        {
            InitializeComponent();

            demoView.GetAllProperties(MyProgressBar);
        }
    }
}