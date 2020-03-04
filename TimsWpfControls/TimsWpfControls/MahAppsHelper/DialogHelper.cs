using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimsWpfControls
{
    public static class DialogHelper
    {
        public static async Task ShowErrorMessage(Exception e, string Header = "Error")
        {
            MetroWindow window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault(x => x.IsActive);
            if (window is null)
            {
                MessageBox.Show(e.Message, Header);
            }
            else
            {
                await window.ShowMessageAsync(Header, e.Message, MessageDialogStyle.Affirmative);
            }
        }
    }
}
