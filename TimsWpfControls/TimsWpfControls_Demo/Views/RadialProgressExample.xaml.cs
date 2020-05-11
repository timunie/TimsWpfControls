using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.PropertyGrid;
using ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors.BrushEditor;
using ICSharpCode.WpfDesign.Designer.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using MahApps.Metro;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for RadialProgressExample.xaml
    /// </summary>
    public partial class RadialProgressExample : UserControl
    {
        private static string xaml = @"
<Button
xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
xmlns:ctrls=""clr-namespace:TimsWpfControls;assembly=TimsWpfControls""
mc:Ignorable=""d""
Width=""200""
Height=""200""
HorizontalAlignment=""Center""
VerticalAlignment=""Center""
x:Name=""progressBar""/>";

        public RadialProgressExample()
        {
            InitializeComponent();

            BasicMetadata.Register();

            var loadSettings = new XamlLoadSettings();
            loadSettings.DesignerAssemblies.Add(this.GetType().Assembly);
            using (var xmlReader = XmlReader.Create(new StringReader(xaml)))
            {
                designSurface.LoadDesigner(xmlReader, loadSettings);
                designSurface.DesignContext.Services.Selection.SetSelectedComponents(new[] { designSurface.DesignContext.RootItem });
            }

            // BrushEditor.SystemBrushes = brushes ;
            Metadata.AddStandardValues(StyleProperty, this.Resources["MahApps.Styles.Button"], this.Resources["MahApps.Styles.Button.Circle"]);
        }
    }
}
