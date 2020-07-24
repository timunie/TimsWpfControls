using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimsWpfControls
{
    /// <summary>
    /// This is a simple Binding Proxy to get rid of the FindAnchestor issues. 
    /// Idea taken from here: https://code.4noobz.net/wpf-mvvm-proxy-binding/
    /// </summary>
    public class DataContextProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new DataContextProxy();
        }

        #endregion

        /// <summary>
        /// Gets or Sets the Data which you want to bind to.
        /// </summary>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(DataContextProxy), new UIPropertyMetadata(null));
    }
}
