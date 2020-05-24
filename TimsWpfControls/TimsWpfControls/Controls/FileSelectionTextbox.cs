using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimsWpfControls
{
    public class FileSelectionTextbox : TextBox
    {
        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(FileSelectionTextbox), new PropertyMetadata("Any File|*.*"));

        // Using a DependencyProperty as the backing store for OpenFileDialog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenFileDialogProperty = DependencyProperty.Register("OpenFileDialog", typeof(OpenFileDialog), typeof(FileSelectionTextbox), new PropertyMetadata(null));

        //// Using a DependencyProperty as the backing store for SelectFileCommand.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SelectFileCommandProperty = DependencyProperty.Register("SelectFileCommand", typeof(ICommand), typeof(FileSelectionTextbox), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for AcceptsFileDrop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptsFileDropProperty = DependencyProperty.Register("AcceptsFileDrop", typeof(bool), typeof(FileSelectionTextbox), new PropertyMetadata(true));

        #region SelectFileCommand

        public static RoutedCommand SelectFileCommand = new RoutedCommand("SelectFile", typeof(FileSelectionTextbox));

        private void SelectFileCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is FileSelectionTextbox fileSelectionTextbox)
            {
                var dlg = fileSelectionTextbox.OpenFileDialog;
                if (dlg.ShowDialog() == true)
                {
                    fileSelectionTextbox.Text = dlg.FileName;
                }
            }
        }

        private void SelectFileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = sender is FileSelectionTextbox;
        }

        #endregion SelectFileCommand

        public override void OnApplyTemplate()
        {
            CommandBindings.Add(new CommandBinding(SelectFileCommand, SelectFileCommand_Execute, SelectFileCommand_CanExecute));
            base.OnApplyTemplate();
        }

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public OpenFileDialog OpenFileDialog
        {
            get { return (OpenFileDialog)GetValue(OpenFileDialogProperty) ?? new OpenFileDialog { Multiselect = false, Filter = Filter }; }
            set { SetValue(OpenFileDialogProperty, value); }
        }

        //public ICommand SelectFileCommand
        //{
        //    get { return (ICommand)GetValue(SelectFileCommandProperty) ?? DefaultSelectFile_Command; }
        //    set { SetValue(SelectFileCommandProperty, value); }
        //}

        public bool AcceptsFileDrop
        {
            get { return (bool)GetValue(AcceptsFileDropProperty); }
            set { SetValue(AcceptsFileDropProperty, value); }
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            if (AcceptsFileDrop)
            {
                e.Effects = e.Data.GetDataPresent("FileDrop") ? DragDropEffects.Copy : DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                base.OnPreviewDragOver(e);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if (AcceptsFileDrop)
            {
                string[] files = (string[])e.Data.GetData("FileDrop");
                Text = files[0];
            }
            else
            {
                base.OnDrop(e);
            }
        }
    }
}