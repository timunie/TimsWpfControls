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

            demoView.AddDemoProperty(CircularProgressBar.ValueProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.MinimumProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.MaximumProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.IsFilledProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.IsIndeterminateProperty, MyProgressBar);

            demoView.AddDemoProperty(CircularProgressBar.ContentProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.ContentStringFormatProperty, MyProgressBar);

            demoView.AddDemoProperty(CircularProgressBar.WidthProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.HeightProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.HorizontalAlignmentProperty, MyProgressBar);
            demoView.AddDemoProperty(CircularProgressBar.VerticalAlignmentProperty, MyProgressBar);
        }
    }
}