using ControlzEx.Theming;
using ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors.BrushEditor;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TimsWpfControls_Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BrushItem[] brushes = Array.Empty<BrushItem>();
            var theme = ThemeManager.Current.DetectTheme();

            foreach (var item in theme.Resources.Keys.Cast<object>()
                .Where(key => theme.Resources[key] is Brush)
                                 .Select(key => key.ToString())
                                 .OrderBy(s => s))
            {
                // BrushEditor.SystemBrushes.Append(new BrushItem() { Name = item, Brush = (Brush)theme.Resources[item] });
            }
        }
    }
}
