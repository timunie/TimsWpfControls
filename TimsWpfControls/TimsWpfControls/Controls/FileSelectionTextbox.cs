using MahApps.Metro.ValueBoxes;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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


        /// <summary>Identifies the <see cref="DialogAddExtension"/> dependency property.</summary>
        public static readonly DependencyProperty DialogAddExtensionProperty =
            DependencyProperty.Register(nameof(DialogAddExtension), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// true if the dialog box adds an extension to a file name if the user omits the extension; otherwise, false. The default value is true.
        /// </summary>
        public bool DialogAddExtension
        {
            get { return (bool)GetValue(DialogAddExtensionProperty); }
            set { SetValue(DialogAddExtensionProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogCheckPathExists"/> dependency property.</summary>
        public static readonly DependencyProperty DialogCheckPathExistsProperty =
            DependencyProperty.Register(nameof(DialogCheckPathExists), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// true if the dialog box displays a warning when the user specifies a path that does not exist; otherwise, false. The default value is true.
        /// </summary>
        public bool DialogCheckPathExists
        {
            get { return (bool)GetValue(DialogCheckPathExistsProperty); }
            set { SetValue(DialogCheckPathExistsProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogCustomPlaces"/> dependency property.</summary>
        public static readonly DependencyProperty DialogCustomPlacesProperty =
            DependencyProperty.Register(nameof(DialogCustomPlaces), typeof(IEnumerable<string>), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a list of custom places. You can either define the GUID or the path of the custom place
        /// </summary>
        public IEnumerable<string> DialogCustomPlaces
        {
            get { return (IEnumerable<string>)GetValue(DialogCustomPlacesProperty); }
            set { SetValue(DialogCustomPlacesProperty, value); }
        }


        /// <summary>Identifies the <see cref="DialogDefaultExt"/> dependency property.</summary>
        public static readonly DependencyProperty DialogDefaultExtProperty =
            DependencyProperty.Register(nameof(DialogDefaultExt), typeof(string), typeof(FileSelectionTextBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// The default file name extension. The returned string does not include the period. The default value is an empty string ("").
        /// </summary>
        public string DialogDefaultExt
        {
            get { return (string)GetValue(DialogDefaultExtProperty); }
            set { SetValue(DialogDefaultExtProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogDereferenceLinks"/> dependency property.</summary>
        public static readonly DependencyProperty DialogDereferenceLinksProperty =
            DependencyProperty.Register(nameof(DialogDereferenceLinks), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// true if the dialog box returns the location of the file referenced by the shortcut; otherwise, false. The default value is true.
        /// </summary>
        public bool DialogDereferenceLinks
        {
            get { return (bool)GetValue(DialogDereferenceLinksProperty); }
            set { SetValue(DialogDereferenceLinksProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogFilterIndex"/> dependency property.</summary>
        public static readonly DependencyProperty DialogFilterIndexProperty =
            DependencyProperty.Register(nameof(DialogFilterIndex), typeof(int), typeof(FileSelectionTextBox), new PropertyMetadata(1));

        /// <summary>
        /// A value containing the index of the filter currently selected in the file dialog box. The default value is 1
        /// </summary>
        public int DialogFilterIndex
        {
            get { return (int)GetValue(DialogFilterIndexProperty); }
            set { SetValue(DialogFilterIndexProperty, value); }
        }




        /// <summary>Identifies the <see cref="DialogInitialDirectory"/> dependency property.</summary>
        public static readonly DependencyProperty DialogInitialDirectoryProperty =
            DependencyProperty.Register(nameof(DialogInitialDirectory), typeof(string), typeof(FileSelectionTextBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// The initial directory displayed by the file dialog box. The default is an empty string ("").
        /// </summary>
        public string DialogInitialDirectory
        {
            get { return (string)GetValue(DialogInitialDirectoryProperty); }
            set { SetValue(DialogInitialDirectoryProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogRestoreDirectory"/> dependency property.</summary>
        public static readonly DependencyProperty DialogRestoreDirectoryProperty =
            DependencyProperty.Register(nameof(DialogRestoreDirectory), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));


        /// <summary>
        /// true if the dialog box restores the current directory to the previously selected directory if the user changed the directory while searching for files; otherwise, false. The default value is false.
        /// </summary>
        public bool DialogRestoreDirectory
        {
            get { return (bool)GetValue(DialogRestoreDirectoryProperty); }
            set { SetValue(DialogRestoreDirectoryProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogSupportMultiDottedExtensions"/> dependency property.</summary>
        public static readonly DependencyProperty DialogSupportMultiDottedExtensionsProperty =
            DependencyProperty.Register(nameof(DialogSupportMultiDottedExtensions), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));


        /// <summary>
        /// true if the dialog box supports multiple file name extensions; otherwise, false. The default is false.
        /// </summary>
        public FileSelectionTextBox DialogSupportMultiDottedExtensions
        {
            get { return (FileSelectionTextBox)GetValue(DialogSupportMultiDottedExtensionsProperty); }
            set { SetValue(DialogSupportMultiDottedExtensionsProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogValidateNames"/> dependency property.</summary>
        public static readonly DependencyProperty DialogValidateNamesProperty =
            DependencyProperty.Register(nameof(DialogValidateNames), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// true if the dialog box accepts only valid Win32 file names; otherwise, false. The default value is true.
        /// </summary>
        public bool DialogValidateNames
        {
            get { return (bool)GetValue(DialogValidateNamesProperty); }
            set { SetValue(DialogValidateNamesProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogSetInitialDirectoryFromSelectedFile"/> dependency property.</summary>
        public static readonly DependencyProperty DialogSetInitialDirectoryFromSelectedFileProperty =
            DependencyProperty.Register(nameof(DialogSetInitialDirectoryFromSelectedFile), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// If this property is set to true, the initial directory will be set from the selected file if any. The default is false.
        /// </summary>
        public bool DialogSetInitialDirectoryFromSelectedFile
        {
            get { return (bool)GetValue(DialogSetInitialDirectoryFromSelectedFileProperty); }
            set { SetValue(DialogSetInitialDirectoryFromSelectedFileProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogInitialFilename"/> dependency property.</summary>
        public static readonly DependencyProperty DialogInitialFilenameProperty =
            DependencyProperty.Register(nameof(DialogInitialFilename), typeof(string), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        /// <summary>
        /// The initial filename when the dialog opens
        /// </summary>
        public string DialogInitialFilename
        {
            get { return (string)GetValue(DialogInitialFilenameProperty); }
            set { SetValue(DialogInitialFilenameProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogSetInitialFilenameFromSelectedFile"/> dependency property.</summary>
        public static readonly DependencyProperty DialogSetInitialFilenameFromSelectedFileProperty =
            DependencyProperty.Register(nameof(DialogSetInitialFilenameFromSelectedFile), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// If <see langword="true"/> the initial filename will be set from current selected filename. The default is <see langword="false"/>
        /// </summary>
        public bool DialogSetInitialFilenameFromSelectedFile
        {
            get { return (bool)GetValue(DialogSetInitialFilenameFromSelectedFileProperty); }
            set { SetValue(DialogSetInitialFilenameFromSelectedFileProperty, value); }
        }




        #region OpenFileDialog Properties

        /// <summary>Identifies the <see cref="DialogCheckFileExists"/> dependency property.</summary>
        public static readonly DependencyProperty DialogCheckFileExistsProperty =
            DependencyProperty.Register(nameof(DialogCheckFileExists), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.TrueBox));


        /// <summary>
        /// true if the dialog box displays a warning when the user specifies a file name that does not exist; otherwise, false. The default value is true.
        /// </summary>
        public bool DialogCheckFileExists
        {
            get { return (bool)GetValue(DialogCheckFileExistsProperty); }
            set { SetValue(DialogCheckFileExistsProperty, value); }
        }



        /// <summary>Identifies the <see cref="DialogShowReadOnly"/> dependency property.</summary>
        public static readonly DependencyProperty DialogShowReadOnlyProperty =
            DependencyProperty.Register(nameof(DialogShowReadOnly), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// true if the dialog box contains a read-only check box; otherwise, false. The default value is false.
        /// </summary>
        public bool DialogShowReadOnly
        {
            get { return (bool)GetValue(DialogShowReadOnlyProperty); }
            set { SetValue(DialogShowReadOnlyProperty, value); }
        }


        /// <summary>Identifies the <see cref="DialogReadOnlyChecked"/> dependency property.</summary>
        public static readonly DependencyProperty DialogReadOnlyCheckedProperty =
            DependencyProperty.Register(nameof(DialogReadOnlyChecked), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// true if the read-only check box is selected; otherwise, false. The default value is false.
        /// </summary>
        public bool DialogReadOnlyChecked
        {
            get { return (bool)GetValue(DialogReadOnlyCheckedProperty); }
            set { SetValue(DialogReadOnlyCheckedProperty, value); }
        }



        #endregion


        #region SaveFileDialog Properties


        /// <summary>Identifies the <see cref="DialogCreatePrompt"/> dependency property.</summary>
        public static readonly DependencyProperty DialogCreatePromptProperty =
            DependencyProperty.Register(nameof(DialogCreatePrompt), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// true if the dialog box prompts the user before creating a file if the user specifies a file name that does not exist; false if the dialog box automatically creates the new file without prompting the user for permission. The default value is false.
        /// </summary>
        public bool DialogCreatePrompt
        {
            get { return (bool)GetValue(DialogCreatePromptProperty); }
            set { SetValue(DialogCreatePromptProperty, value); }
        }


        /// <summary>Identifies the <see cref="DialogOverwritePrompt"/> dependency property.</summary>
        public static readonly DependencyProperty DialogOverwritePromptProperty =
            DependencyProperty.Register(nameof(DialogOverwritePrompt), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// true if the dialog box prompts the user before overwriting an existing file if the user specifies a file name that already exists; false if the dialog box automatically overwrites the existing file without prompting the user for permission. The default value is true.
        /// </summary>
        public bool DialogOverwritePrompt
        {
            get { return (bool)GetValue(DialogOverwritePromptProperty); }
            set { SetValue(DialogOverwritePromptProperty, value); }
        }

        #endregion

        #region FolderBrowser

        /// <summary>Identifies the <see cref="FolderDialogDescription"/> dependency property.</summary>
        public static readonly DependencyProperty FolderDialogDescriptionProperty =
            DependencyProperty.Register(nameof(FolderDialogDescription), typeof(string), typeof(FileSelectionTextBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// The description to display. The default is an empty string ("").
        /// </summary>
        public string FolderDialogDescription
        {
            get { return (string)GetValue(FolderDialogDescriptionProperty); }
            set { SetValue(FolderDialogDescriptionProperty, value); }
        }



        /// <summary>Identifies the <see cref="FolderDialogRootFolder"/> dependency property.</summary>
        public static readonly DependencyProperty FolderDialogRootFolderProperty =
            DependencyProperty.Register(nameof(FolderDialogRootFolder), typeof(Environment.SpecialFolder), typeof(FileSelectionTextBox), new PropertyMetadata(Environment.SpecialFolder.Desktop));


        /// <summary>
        /// One of the <see cref="Environment.SpecialFolder"/> values. The default is Desktop.
        /// </summary>
        public Environment.SpecialFolder FolderDialogRootFolder
        {
            get { return (Environment.SpecialFolder)GetValue(FolderDialogRootFolderProperty); }
            set { SetValue(FolderDialogRootFolderProperty, value); }
        }


        /// <summary>Identifies the <see cref="FolderDialogSelectedPath"/> dependency property.</summary>
        public static readonly DependencyProperty FolderDialogSelectedPathProperty =
            DependencyProperty.Register(nameof(FolderDialogSelectedPath), typeof(string), typeof(FileSelectionTextBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// The path of the folder first selected in the dialog box or the last folder selected by the user. The default is an empty string ("").
        /// </summary>
        public string FolderDialogSelectedPath
        {
            get { return (string)GetValue(FolderDialogSelectedPathProperty); }
            set { SetValue(FolderDialogSelectedPathProperty, value); }
        }



        /// <summary>Identifies the <see cref="FolderDialogShowNewFolderButton"/> dependency property.</summary>
        public static readonly DependencyProperty FolderDialogShowNewFolderButtonProperty =
            DependencyProperty.Register(nameof(FolderDialogShowNewFolderButton), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// true if the New Folder button is shown in the dialog box; otherwise, false. The default is true.
        /// </summary>
        public bool FolderDialogShowNewFolderButton
        {
            get { return (bool)GetValue(FolderDialogShowNewFolderButtonProperty); }
            set { SetValue(FolderDialogShowNewFolderButtonProperty, value); }
        }



        /// <summary>Identifies the <see cref="FolderDialogUseDescriptionForTitle"/> dependency property.</summary>
        public static readonly DependencyProperty FolderDialogUseDescriptionForTitleProperty =
            DependencyProperty.Register(nameof(FolderDialogUseDescriptionForTitle), typeof(bool), typeof(FileSelectionTextBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// true if the value of the Description property is used as the dialog title; false if the value is added as additional text to the dialog. The default is false.
        /// </summary>
        public bool FolderDialogUseDescriptionForTitle
        {
            get { return (bool)GetValue(FolderDialogUseDescriptionForTitleProperty); }
            set { SetValue(FolderDialogUseDescriptionForTitleProperty, value); }
        }


        #endregion

        #region Dialog Type

        // Using a DependencyProperty as the backing store for FileDialogType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileDialogTypeProperty =
            DependencyProperty.Register("FileDialogType", typeof(FileDialogType), typeof(FileSelectionTextBox), new PropertyMetadata(FileDialogType.OpenFileDialog));

        
        public FileDialogType FileDialogType
        {
            get { return (FileDialogType)GetValue(FileDialogTypeProperty); }
            set { SetValue(FileDialogTypeProperty, value); }
        }
        
        #endregion

        #region SelectFileButton

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


        /// <summary>Identifies the <see cref="ButtonTooltip"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonTooltipProperty =
            DependencyProperty.Register(nameof(ButtonTooltip), typeof(object), typeof(FileSelectionTextBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="ToolTip"/> for the button that is used to select the file.
        /// </summary>
        public object ButtonTooltip
        {
            get { return (object)GetValue(ButtonTooltipProperty); }
            set { SetValue(ButtonTooltipProperty, value); }
        }



        #endregion

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
            if (parameter is FileSelectionTextBox fstb)
            {
                string fileName;

                switch (fstb.FileDialogType)
                {
                    case FileDialogType.OpenFileDialog:
                        var openDialog = new OpenFileDialog
                        {
                            AddExtension = fstb.DialogAddExtension,
                            CheckFileExists = fstb.DialogCheckFileExists,
                            CheckPathExists = fstb.DialogCheckPathExists,
                            DefaultExt = fstb.DialogDefaultExt,
                            DereferenceLinks = fstb.DialogDereferenceLinks,
                            FileName = fstb.DialogInitialFilename,
                            Filter = fstb.FilterString,
                            FilterIndex = fstb.DialogFilterIndex,
                            InitialDirectory = fstb.DialogInitialDirectory,
                            Multiselect = false,
                            ReadOnlyChecked = fstb.DialogReadOnlyChecked,
                            RestoreDirectory = fstb.DialogRestoreDirectory,
                            ShowReadOnly = fstb.DialogShowReadOnly,
                            Title = fstb.DialogTitle,
                            ValidateNames = fstb.DialogValidateNames
                        };

                        if (!(fstb.DialogCustomPlaces is null))
                        {
                            foreach (var customPlace in fstb.DialogCustomPlaces)
                            {
                                openDialog.CustomPlaces.Add(new FileDialogCustomPlace(customPlace));
                            }
                        }

                        if (fstb.DialogSetInitialDirectoryFromSelectedFile && !string.IsNullOrWhiteSpace(fstb.Text))
                        {
                            openDialog.InitialDirectory = Path.GetDirectoryName(fstb.Text);
                        }

                        if (fstb.DialogSetInitialFilenameFromSelectedFile && !string.IsNullOrWhiteSpace(fstb.Text))
                        {
                            openDialog.FileName = Path.GetFileName(fstb.Text);
                        }

                        if (openDialog.ShowDialog() == true)
                        {
                            fileName = fstb.TryGetUncPath ? LocalToUNC(openDialog.FileName) : openDialog.FileName;
                            fstb.SetFileNameInternally(fileName);
                        }
                        break;


                    case FileDialogType.SaveFileDialog:
                        var saveDialog = new SaveFileDialog
                        {
                            AddExtension = fstb.DialogAddExtension,
                            CheckFileExists = fstb.DialogCheckFileExists,
                            CheckPathExists = fstb.DialogCheckPathExists,
                            DefaultExt = fstb.DialogDefaultExt,
                            DereferenceLinks = fstb.DialogDereferenceLinks,
                            FileName = Path.GetFileName(fstb.Text ?? string.Empty),
                            Filter = fstb.FilterString,
                            FilterIndex = fstb.DialogFilterIndex,
                            InitialDirectory = fstb.DialogInitialDirectory,
                            RestoreDirectory = fstb.DialogRestoreDirectory,
                            Title = fstb.DialogTitle,
                            ValidateNames = fstb.DialogValidateNames,
                            CreatePrompt = fstb.DialogCreatePrompt,
                            OverwritePrompt = fstb.DialogOverwritePrompt
                        };

                        if (!(fstb.DialogCustomPlaces is null))
                        {
                            foreach (var customPlace in fstb.DialogCustomPlaces)
                            {
                                saveDialog.CustomPlaces.Add(new FileDialogCustomPlace(customPlace));
                            }
                        }

                        if (fstb.DialogSetInitialDirectoryFromSelectedFile && !string.IsNullOrWhiteSpace(fstb.Text))
                        {
                            saveDialog.InitialDirectory = Path.GetDirectoryName(fstb.Text);
                        }

                        if (fstb.DialogSetInitialFilenameFromSelectedFile && !string.IsNullOrWhiteSpace(fstb.Text))
                        {
                            saveDialog.FileName = Path.GetFileName(fstb.Text);
                        }

                        if (saveDialog.ShowDialog() == true)
                        {
                            fileName = fstb.TryGetUncPath ? LocalToUNC(saveDialog.FileName) : saveDialog.FileName;
                            fstb.SetFileNameInternally(fileName);
                        }
                        break;


                    case FileDialogType.FolderBrowserDialog:
                        var folderDialog = new VistaFolderBrowserDialog
                        {
                            Description = fstb.FolderDialogDescription,
                            RootFolder = fstb.FolderDialogRootFolder,
                            SelectedPath = fstb.FolderDialogSelectedPath,
                            ShowNewFolderButton = fstb.FolderDialogShowNewFolderButton,
                            UseDescriptionForTitle = fstb.FolderDialogUseDescriptionForTitle
                        };
                           
                        if (fstb.DialogSetInitialDirectoryFromSelectedFile && !string.IsNullOrWhiteSpace(fstb.Text))
                        {
                            folderDialog.SelectedPath = fstb.Text;
                        }

                        if (folderDialog.ShowDialog() == true)
                        {
                            fileName = fstb.TryGetUncPath ? LocalToUNC(folderDialog.SelectedPath) : folderDialog.SelectedPath;
                            fstb.SetFileNameInternally(fileName);
                        }
                        break;
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

                SetFileNameInternally(fileName);

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

        #region ValidateFileName event

        /// <summary>
        /// This event will be raised when the <see cref="TextBox.TextProperty"/> is set by either using a <see cref="FileDialog"/> or <see cref="DragDrop.DropEvent"/>
        /// </summary>
        public static readonly RoutedEvent ValidatingFileNameEvent = EventManager.RegisterRoutedEvent(
            nameof(ValidatingFileName), 
            RoutingStrategy.Bubble, 
            typeof(ValidatingFileEventHandler), 
            typeof(FileSelectionTextBox));

        // Provide CLR accessors for the event
        public event ValidatingFileEventHandler ValidatingFileName
        {
            add { AddHandler(ValidatingFileNameEvent, value); }
            remove { RemoveHandler(ValidatingFileNameEvent, value); }
        }

        private void SetFileNameInternally(string fileName)
        {
            var eventArgs = new ValidatingFileEventArgs(fileName, ValidatingFileNameEvent);

            RaiseEvent(eventArgs);

            if (eventArgs.Handled)
            {
                return;
            }
            else if (eventArgs.IsValid)
            {
                SetValue(TextProperty, eventArgs.FileName);
                GetBindingExpression(TextProperty)?.UpdateSource();
            }
            else if (!eventArgs.IsValid && !string.IsNullOrEmpty(eventArgs.ValidFileName))
            {
                SetValue(TextProperty, eventArgs.ValidFileName);
                GetBindingExpression(TextProperty)?.UpdateSource();
            }
        }

        #endregion

    }

    public enum FileDialogType
    {
        OpenFileDialog,
        SaveFileDialog, 
        FolderBrowserDialog
    }

    public delegate void ValidatingFileEventHandler(object sender, ValidatingFileEventArgs e);

    public class ValidatingFileEventArgs : RoutedEventArgs
    {
        public ValidatingFileEventArgs(string fileName) : base()
        {
            FileName = fileName;
        }

        public ValidatingFileEventArgs(string fileName, RoutedEvent ValidatingFileEvent) : base(ValidatingFileEvent)
        {
            FileName = fileName;
        }

        public ValidatingFileEventArgs(string fileName, RoutedEvent ValidatingFileEvent, object source) : base(ValidatingFileEvent, source)
        {
            FileName = fileName;
        }

        /// <summary>
        /// The FileName passed from the <see cref="FileSelectionTextBox"/>
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets or Sets if the selected file is valid. If the provided <see cref="FileName"/> is not valid set this parameter to false.
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// If <see cref="IsValid"/> is set to false the <see cref="FileName"/> will be replaced by this string. If this parameter is <c>null</c> nothing will be set.
        /// </summary>
        public string ValidFileName { get; set; }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            ValidatingFileEventHandler handler;

            handler = (ValidatingFileEventHandler)genericHandler;
            handler(genericTarget, this);
        }
    }
}