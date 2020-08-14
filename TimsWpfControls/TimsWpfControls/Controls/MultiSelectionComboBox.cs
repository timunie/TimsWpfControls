using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using TimsWpfControls.Helper;

namespace TimsWpfControls
{
    [TemplatePart(Name = nameof(PART_PopupListBox), Type = typeof(ListBox))]
    public class MultiSelectionComboBox : ComboBox
    {

        #region Constructors

        static MultiSelectionComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(typeof(MultiSelectionComboBox)));
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Members
        // 
        //-------------------------------------------------------------------

        #region private Members

        private ListBox PART_PopupListBox;
        private TextBox PART_EditableTextBox;
        private bool isUpdatingText = false;

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Properties
        // 
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>
        ///     SelectionMode DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
                DependencyProperty.Register(
                        "SelectionMode",
                        typeof(SelectionMode),
                        typeof(MultiSelectionComboBox),
                        new FrameworkPropertyMetadata(
                                SelectionMode.Single,
                                new PropertyChangedCallback(OnSelectionModeChanged)),
                        new ValidateValueCallback(IsValidSelectionMode));

        /// <summary>
        ///     Indicates the selection behavior for the ListBox.
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectionComboBox multiSelectionComboBox = (MultiSelectionComboBox)d;
            multiSelectionComboBox.ValidateSelectionMode(multiSelectionComboBox.SelectionMode);
        }

        private static object OnGetSelectionMode(DependencyObject d)
        {
            return ((MultiSelectionComboBox)d).SelectionMode;
        }


        private static bool IsValidSelectionMode(object o)
        {
            SelectionMode value = (SelectionMode)o;
            return value == SelectionMode.Single
                || value == SelectionMode.Multiple
                || value == SelectionMode.Extended;
        }


        private bool CanSelectMultiple;
        private void ValidateSelectionMode(SelectionMode mode)
        {
            CanSelectMultiple = (mode != SelectionMode.Single);
        }


        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectionComboBox), new PropertyMetadata((IList)null));

        /// <summary>
        /// The currently selected items.
        /// </summary>
        [Bindable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList SelectedItems
        {
            get
            {
                return PART_PopupListBox?.SelectedItems;
            }
        }


        /// <summary>
        /// Separator DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register("Separator", typeof(object), typeof(MultiSelectionComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the Separator Content. ToString() will be used if the ComboBox is editable.
        /// </summary>
        public object Separator
        {
            get { return (object)GetValue(SeparatorProperty); }
            set { SetValue(SeparatorProperty, value); }
        }


        /// <summary>
        /// SeparatorTemplate DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SeparatorTemplateProperty = DependencyProperty.Register("SeparatorTemplate", typeof(DataTemplate), typeof(MultiSelectionComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the SeparatorTemplate. Gets only applied if the MultiselectionComboBox is not editable
        /// </summary>
        public DataTemplate SeparatorTemplate
        {
            get { return (DataTemplate)GetValue(SeparatorTemplateProperty); }
            set { SetValue(SeparatorTemplateProperty, value); }
        }


        /// <summary>
        ///     HasCustomText DependencyProperty
        /// </summary>
        public static readonly DependencyProperty HasCustomTextProperty = DependencyProperty.Register("HasCustomText", typeof(bool), typeof(MultiSelectionComboBox), new PropertyMetadata(false));

        /// <summary>
        /// Indicates if the text is userdefined
        /// </summary>
        public bool HasCustomText
        {
            get { return (bool)GetValue(HasCustomTextProperty); }
        }

        /// <summary>
        /// Updates the Text of the editable Textbox.
        /// Sets the custom Text if any otherwise the concatenated string.
        /// </summary>
        public void UpdateEditableText()
        {
            if (PART_EditableTextBox is null || SelectedItems is null)
                return;

            isUpdatingText = true;
            if (string.IsNullOrEmpty(Text) && SelectionMode != SelectionMode.Single)
            {
                var items = ((IEnumerable<object>)SelectedItems).OrderBy(o => Items.IndexOf(o));
                PART_EditableTextBox.SetCurrentValue(TextBox.TextProperty, string.Join(Separator.ToString(), items));
                SetCurrentValue(HasCustomTextProperty, false);
            }
            else if ((string.IsNullOrEmpty(Text) && SelectionMode == SelectionMode.Single))
            {
                PART_EditableTextBox.SetCurrentValue(TextBox.TextProperty, SelectedItem);
                SetCurrentValue(HasCustomTextProperty, false);
            }
            else
            {
                PART_EditableTextBox.SetCurrentValue(TextBox.TextProperty, Text);
                SetCurrentValue(HasCustomTextProperty, true);
            }
            isUpdatingText = false;
        }

        /// <summary>
        /// DisabledPopupOverlayContent DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DisabledPopupOverlayContentProperty = DependencyProperty.Register("DisabledPopupOverlayContent", typeof(object), typeof(MultiSelectionComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the DisabledPopupOverlayContent
        /// </summary>
        public object DisabledPopupOverlayContent
        {
            get { return (object)GetValue(DisabledPopupOverlayContentProperty); }
            set { SetValue(DisabledPopupOverlayContentProperty, value); }
        }


        /// <summary>
        /// DisabledPopupOverlayContentTemplate DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DisabledPopupOverlayContentTemplateProperty = DependencyProperty.Register("DisabledPopupOverlayContentTemplate", typeof(DataTemplate), typeof(MultiSelectionComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the DisabledPopupOverlayContentTemplate
        /// </summary>
        public DataTemplate DisabledPopupOverlayContentTemplate
        {
            get { return (DataTemplate)GetValue(DisabledPopupOverlayContentTemplateProperty); }
            set { SetValue(DisabledPopupOverlayContentTemplateProperty, value); }
        }

        /// <summary>
        /// SelectedItemsTemplate DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SelectedItemsTemplateProperty = DependencyProperty.Register(nameof(SelectedItemsTemplate), typeof(DataTemplate), typeof(MultiSelectionComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the SelectedItemsTemplate
        /// </summary>
        public DataTemplate SelectedItemsTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemsTemplateProperty); }
            set { SetValue(SelectedItemsTemplateProperty, value); }
        }

        /// <summary>
        /// SelectedItemsTemplateSelector DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SelectedItemsTemplateSelectorProperty = DependencyProperty.Register(nameof(SelectedItemsTemplateSelector), typeof(DataTemplateSelector), typeof(MultiSelectionComboBox), new PropertyMetadata(null));


        

        /// <summary>
        /// Gets or Sets the SelectedItemsTemplateSelector
        /// </summary>
        public DataTemplateSelector SelectedItemsTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectedItemsTemplateSelectorProperty); }
            set { SetValue(SelectedItemsTemplateSelectorProperty, value); }
        }

        #endregion

        #region Commands

        // Clear Text Command
        public static RoutedUICommand ClearContentCommand { get; } = new RoutedUICommand("ClearContent", nameof(ClearContentCommand), typeof(MultiSelectionComboBox));

        private void ExecutedClearContentCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MultiSelectionComboBox multiSelectionCombo)
            {

                if (multiSelectionCombo.Text != null)
                {
                    multiSelectionCombo.SetCurrentValue(TextProperty, null);
                }
                else
                {
                    if (SelectionMode == SelectionMode.Single)
                    {
                        multiSelectionCombo.SelectedItem = null;
                        return;
                    }
                    multiSelectionCombo.SelectedItems.Clear();
                }
            }
        }

        private void CanExecuteClearContentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (sender is MultiSelectionComboBox multiSelectionComboBox)
            {
                e.CanExecute = multiSelectionComboBox.Text != null || multiSelectionComboBox.SelectedItems?.Count > 0;
            }
        }


        public static RoutedUICommand RemoveItemCommand { get; } = new RoutedUICommand("Remove item", nameof(RemoveItemCommand), typeof(MultiSelectionComboBox));

        private void RemoveItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MultiSelectionComboBox multiSelectionCombo && multiSelectionCombo.SelectedItems.Contains(e.Parameter))
            {
                if (multiSelectionCombo.SelectionMode == SelectionMode.Single)
                {
                    multiSelectionCombo.SelectedItem = null;
                    return;
                }
                multiSelectionCombo.SelectedItems.Remove(e.Parameter);
            }
        }

        private void RemoveItemCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (sender is MultiSelectionComboBox multiSelectionComboBox)
            {
                e.CanExecute = e.Parameter != null;
            }
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_EditableTextBox = GetTemplateChild(nameof(PART_EditableTextBox)) as TextBox;
            PART_EditableTextBox.TextChanged += PART_EditableTextBox_TextChanged;

            PART_PopupListBox = GetTemplateChild(nameof(PART_PopupListBox)) as ListBox;
            PART_PopupListBox.SelectionChanged += PART_PopupListBox_SelectionChanged;

            CommandBindings.Add(new CommandBinding(ClearContentCommand, ExecutedClearContentCommand, CanExecuteClearContentCommand));
            CommandBindings.Add(new CommandBinding(RemoveItemCommand, RemoveItemCommand_Executed, RemoveItemCommand_CanExecute));

            // Do update the text 
            UpdateEditableText();
        }

        private void PART_PopupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEditableText();
            SetCurrentValue(SelectedItemsProperty, PART_PopupListBox.SelectedItems);
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            if (!isUpdatingText)
            {
                var text = PART_EditableTextBox?.Text;

                if (text == string.Join(Separator.ToString(), (IEnumerable<object>)SelectedItems))
                {
                    Text = null;
                }
                else
                {
                    Text = text;
                }
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateEditableText();
        }


        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (!IsLoaded)
            {
                Loaded += MultiSelectionComboBox_Loaded;
                return; 
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        PART_PopupListBox.Items.Add(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        PART_PopupListBox.Items.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // TODO Add Handler
                    break;
                case NotifyCollectionChangedAction.Move:
                    // TODO Add Handler
                    break;
                case NotifyCollectionChangedAction.Reset:
                    PART_PopupListBox.Items.Clear();
                    foreach (var item in Items)
                    {
                        PART_PopupListBox.Items.Add(item);
                    }
                    break;
                default:
                    break;
            }
        }

        private void MultiSelectionComboBox_Loaded(object sender, EventArgs e)
        {
            Initialized -= MultiSelectionComboBox_Loaded;

            PART_PopupListBox.Items.Clear();
            foreach (var item in Items)
            {
                PART_PopupListBox.Items.Add(item);
            }
        }
        #endregion

        #region Events

        private void PART_EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isUpdatingText)
            {
                var text = PART_EditableTextBox?.Text;

                if (text == string.Join(Separator.ToString(), (IEnumerable<object>)SelectedItems))
                {
                    SetCurrentValue(TextProperty, null);
                }
                else
                {
                    SetCurrentValue(TextProperty, text);
                }

                UpdateEditableText();
            }
        }

        #endregion
    }


    /// <summary>
    /// THE BACKUP
    /// </summary>
    //[TemplatePart (Name = nameof(PART_EditableTextBox), Type = typeof(TextBox))]
    //[TemplatePart (Name = nameof(PART_Popup), Type = typeof(Popup))]
    //[TemplatePart (Name = nameof(PART_PopupItemsPresenter), Type = typeof(ItemsPresenter))]
    //public class MultiSelectionComboBox : ListBox
    //{
    //    private TextBox PART_EditableTextBox;
    //    private Popup PART_Popup;
    //    private ItemsPresenter PART_PopupItemsPresenter;

    //    static MultiSelectionComboBox()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(typeof(MultiSelectionComboBox)));
    //        EventManager.RegisterClassHandler(typeof(MultiSelectionComboBox), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
    //        EventManager.RegisterClassHandler(typeof(MultiSelectionComboBox), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true); // call us even if the transparent button in the style gets the click.
    //    }

    //    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty TextProperty =
    //        DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectionComboBox), 
    //            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));




    //    private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is MultiSelectionComboBox multiSelectionComboBox)
    //        {
    //            if ((bool)e.NewValue)
    //            {
    //                multiSelectionComboBox.RaiseEvent(new RoutedEventArgs(DropDownOpenedEvent));

    //                multiSelectionComboBox.Focus();

    //                Mouse.Capture(multiSelectionComboBox, CaptureMode.SubTree);

    //                multiSelectionComboBox.Dispatcher.BeginInvoke(
    //                   DispatcherPriority.Send,
    //                   (DispatcherOperationCallback)delegate (object arg)
    //                   {
    //                       MultiSelectionComboBox mscb = (MultiSelectionComboBox)arg;

    //                       var item = multiSelectionComboBox.SelectedItem ?? (mscb.HasItems ? mscb.Items[0] : null);
    //                       if (item != null)
    //                       {
    //                           var listBoxItem = mscb.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
    //                           listBoxItem?.Focus();
    //                           ControlzEx.KeyboardNavigationEx.Focus(listBoxItem);
    //                       }

    //                       return null;
    //                   }, multiSelectionComboBox);
    //            }
    //            else
    //            {
    //                multiSelectionComboBox.RaiseEvent(new RoutedEventArgs(DropDownClosedEvent));
    //                if (Mouse.Captured == multiSelectionComboBox)
    //                {
    //                    Mouse.Capture(null);
    //                }
    //            }
    //        }
    //    }




    //    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is MultiSelectionComboBox multiSelectionComboBox)
    //        {
    //            multiSelectionComboBox.UpdateEditableText();
    //        }
    //    }




    //    #region DataTemplates



    //    #endregion

    //    #region Override


    //    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    //    {
    //        base.OnItemsSourceChanged(oldValue, newValue);
    //        UpdateEditableText();
    //    }

    //    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    //    {
    //        // Ignore the first mouse button up if we haven't gone over the popup yet
    //        // And ignore all mouse ups over the items host.
    //        if (!PART_Popup.IsMouseOver)
    //        {
    //            if (IsDropDownOpen)
    //            {
    //                Close();
    //                e.Handled = true;
    //            }
    //        }

    //        base.OnMouseLeftButtonUp(e);
    //    }

    //    private void Close()
    //    {
    //        if (IsDropDownOpen)
    //        {
    //            SetCurrentValue(IsDropDownOpenProperty, false);
    //        }
    //    }

    //    private static void OnLostMouseCapture(object sender, MouseEventArgs e)
    //    {
    //        MultiSelectionComboBox multiSelectionComboBox = (MultiSelectionComboBox)sender;

    //        // ISSUE (jevansa) -- task 22022:
    //        //        We need a general mechanism to do this, or at the very least we should
    //        //        share it amongst the controls which need it (Popup, MenuBase, ComboBox).
    //        if (Mouse.Captured != multiSelectionComboBox)
    //        {
    //            if (e.OriginalSource == multiSelectionComboBox)
    //            {
    //                // If capture is null or it's not below the combobox, close.
    //                // More workaround for task 22022 -- check if it's a descendant (following Logical links too)
    //                if (Mouse.Captured == null || !(Mouse.Captured as DependencyObject).IsDescendantOf(multiSelectionComboBox))
    //                {
    //                    multiSelectionComboBox.Close();
    //                }
    //            }
    //            else
    //            {
    //                if ((e.OriginalSource as DependencyObject).IsDescendantOf(multiSelectionComboBox))
    //                {
    //                    // Take capture if one of our children gave up capture (by closing their drop down)
    //                    if (multiSelectionComboBox.IsDropDownOpen && Mouse.Captured == null)
    //                    {
    //                        Mouse.Capture(multiSelectionComboBox, CaptureMode.SubTree);
    //                        e.Handled = true;
    //                    }
    //                }
    //                else
    //                {
    //                    multiSelectionComboBox.Close();
    //                }
    //            }
    //            e.Handled = true;
    //        }
    //    }
    //    #endregion
    //}
}
