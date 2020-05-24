using System.Windows.Controls;
using TimsWpfControls;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for RadialProgressExample.xaml
    /// </summary>
    public partial class RadialProgressExample : UserControl
    {
        private static readonly string DemoXaml = @"
<ctrls:CircularProgressBar Value=""[Value]""
                           Minimum=""0""
                           Width=""[Width]""
                           Height=""[Height]""
                           ContentStringFormat=""0' %'""
                           Maximum=""100"" />
";
        public RadialProgressExample()
        {
            InitializeComponent();
            demoView.AddDemoProperty(CircularProgressBar.WidthProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.HeightProperty, MyProgressBar);

            demoView.ExampleXaml = DemoXaml;
        }
    }
}