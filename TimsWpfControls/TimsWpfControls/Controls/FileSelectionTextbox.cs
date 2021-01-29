using MahApps.Metro.ValueBoxes;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TimsWpfControls.Model;


namespace TimsWpfControls
{

    [StyleTypedProperty (Property = nameof(ButtonStyle), StyleTargetType = (typeof(Button)))]
    public class FileSelectionTextBox : TextBox
    {
        static FileSelectionTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileSelectionTextBox), new FrameworkPropertyMetadata(typeof(FileSelectionTextBox)));
        }



        public static readonly DependencyProperty FilterStringProperty =
            DependencyProperty.Register("FilterString", typeof(string), typeof(FileSelectionTextBox), new PropertyMetadata("Any File (*.*)|*.*"));

        /// <summary>
        /// Gets or Sets the <see cref="FileDialog.Filter"/> which is used to filter the <see cref="FileDialog"/>
        /// </summary>
        public string FilterString
        {
            get { return (string)GetValue(FilterStringProperty); }
            set { SetValue(FilterStringProperty, value); }
        }



        public static readonly DependencyProperty DialogTitleProperty =
            DependencyProperty.Register("DialogTitle", typeof(string), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="FileDialog.Title"/> which will be shown on the <see cref="FileDialog"/>
        /// </summary>
        public string DialogTitle
        {
            get { return (string)GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }



        // Using a DependencyProperty as the backing store for FileDialogType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileDialogTypeProperty =
            DependencyProperty.Register("FileDialogType", typeof(FileDialogType), typeof(FileSelectionTextBox), new PropertyMetadata(FileDialogType.OpenFileDialog));


        public FileDialogType FileDialogType
        {
            get { return (FileDialogType)GetValue(FileDialogTypeProperty); }
            set { SetValue(FileDialogTypeProperty, value); }
        }


        // Using a DependencyProperty as the backing store for ButtonWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register("ButtonWidth", typeof(double), typeof(FileSelectionTextBox), new PropertyMetadata(20d));

        /// <summary>
        /// Gets or Sets the width of the selection button
        /// </summary>
        public double ButtonWidth
        {
            get { return (double)GetValue(ButtonWidthProperty); }
            set { SetValue(ButtonWidthProperty, value); }
        }



        // Using a DependencyProperty as the backing store for ButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonContentProperty =
            DependencyProperty.Register("ButtonContent", typeof(object), typeof(FileSelectionTextBox), new PropertyMetadata("..."));

        public object ButtonContent
        {
            get { return (object)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }




        // Using a DependencyProperty as the backing store for ButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }


        // Using a DependencyProperty as the backing store for ButtonContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonContentTemplateProperty =
            DependencyProperty.Register("ButtonContentTemplate", typeof(DataTemplate), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        public DataTemplate ButtonContentTemplate
        {
            get { return (DataTemplate)GetValue(ButtonContentTemplateProperty); }
            set { SetValue(ButtonContentTemplateProperty, value); }
        }



        #region SelectFileCommand


        // Using a DependencyProperty as the backing store for SelectFileCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectFileCommandProperty =
            DependencyProperty.Register("SelectFileCommand", typeof(ICommand), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        public ICommand SelectFileCommand
        {
            get { return (ICommand)GetValue(SelectFileCommandProperty); }
            set { SetValue(SelectFileCommandProperty, value); }
        }


        // Using a DependencyProperty as the backing store for SelectFileCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectFileCommandParameterProperty =
            DependencyProperty.Register("SelectFileCommandParameter", typeof(object), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        public object SelectFileCommandParameter
        {
            get { return (object)GetValue(SelectFileCommandParameterProperty); }
            set { SetValue(SelectFileCommandParameterProperty, value); }
        }



        // Using a DependencyProperty as the backing store for SelectFileCommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectFileCommandTargetProperty =
            DependencyProperty.Register("SelectFileCommandTarget", typeof(IInputElement), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        public IInputElement SelectFileCommandTarget
        {
            get { return (IInputElement)GetValue(SelectFileCommandTargetProperty); }
            set { SetValue(SelectFileCommandTargetProperty, value); }
        }



        public static RelayCommand DefaultSelectFileCommand { get; } = new RelayCommand
            (
                (parameter) => DefaultSelectFileCommand_Execute(parameter)
            );

        private static void DefaultSelectFileCommand_Execute(object parameter)
        {
            if (parameter is FileSelectionTextBox fileSelectionTextbox)
            {
                FileDialog fileDialog;

                fileDialog = (fileSelectionTextbox.FileDialogType) switch
                {
                    FileDialogType.OpenFileDialog => new OpenFileDialog() { Multiselect = false },
                    FileDialogType.SaveFileDialog => new SaveFileDialog(),
                    _ => new OpenFileDialog()
                };

                fileDialog.Title = fileSelectionTextbox.DialogTitle;
                fileDialog.Filter = fileSelectionTextbox.FilterString;

                if (!string.IsNullOrEmpty(fileSelectionTextbox.Text))
                {
                    fileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(fileSelectionTextbox.Text);
                }

                if (fileDialog.ShowDialog() == true)
                {
                    var fileName = fileSelectionTextbox.TryGetUncPath ? LocalToUNC(fileDialog.FileName) : fileDialog.FileName;

                    fileSelectionTextbox.SetCurrentValue(TextProperty, fileName);
                }
            }
        }

        #endregion


        #region FileDrop

        private void FileSelectionTextbox_PreviewDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length == 1)
            {
                var fileName = TryGetUncPath ? LocalToUNC(files[0]) : files[0];
                SetCurrentValue(TextProperty, fileName);
                e.Handled = true;
            }
        }

        private void FileSelectionTextbox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
                e.Handled = true;
            }
        }

        private void FileSelectionTextbox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
                e.Handled = true;
            }
        }
        #endregion


        #region Applay Tempalte

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PreviewDragEnter += FileSelectionTextbox_PreviewDragEnter;
            PreviewDragOver += FileSelectionTextbox_PreviewDragOver;
            PreviewDrop += FileSelectionTextbox_PreviewDrop;
        }

        #endregion


        #region Handle UNC Path



        public bool TryGetUncPath
        {
            get { return (bool)GetValue(TryGetUncPathProperty); }
            set { SetValue(TryGetUncPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TryGetUncPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TryGetUncPathProperty =
            DependencyProperty.Register("TryGetUncPath", typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        [DllImport("mpr.dll")]
        static extern int WNetGetUniversalNameA(string lpLocalPath, int dwInfoLevel, IntPtr lpBuffer, ref int lpBufferSize);

        // I think max length for UNC is actually 32,767
        static string LocalToUNC(string localPath, int maxLen = 2000)
        {
            IntPtr lpBuff;

            // Allocate the memory
            try
            {
                lpBuff = Marshal.AllocHGlobal(maxLen);
            }
            catch (OutOfMemoryException)
            {
                return null;
            }

            try
            {
                int res = WNetGetUniversalNameA(localPath, 1, lpBuff, ref maxLen);

                if (res != 0)
                    return localPath;

                // lpbuff is a structure, whose first element is a pointer to the UNC name (just going to be lpBuff + sizeof(int))
                return Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(lpBuff));
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(lpBuff);
            }
        }

        #endregion

    }

    public enum FileDialogType
    {
        OpenFileDialog,
        SaveFileDialog
    }
}