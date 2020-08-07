using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using TimsWpfControls.Helper;

namespace TimsWpfControls
{
    [TemplatePart (Name = nameof(PART_EditableTextBox), Type = typeof(TextBox))]
    [TemplatePart (Name = nameof(PART_Popup), Type = typeof(Popup))]
    [TemplatePart (Name = nameof(PART_PopupItemsPresenter), Type = typeof(ItemsPresenter))]
    public class MultiSelectionComboBox : ListBox
    {
        private TextBox PART_EditableTextBox;
        private Popup PART_Popup;
        private ItemsPresenter PART_PopupItemsPresenter;

        static MultiSelectionComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(typeof(MultiSelectionComboBox)));
            EventManager.RegisterClassHandler(typeof(MultiSelectionComboBox), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(MultiSelectionComboBox), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true); // call us even if the transparent button in the style gets the click.
        }
        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(MultiSelectionComboBox), new PropertyMetadata(true));

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(MultiSelectionComboBox), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged));

        // Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(MultiSelectionComboBox), new PropertyMetadata(SystemParameters.PrimaryScreenHeight / 3));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectionComboBox), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));


        // Using a DependencyProperty as the backing store for HasCustomText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasCustomTextProperty = DependencyProperty.Register("HasCustomText", typeof(bool), typeof(MultiSelectionComboBox), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for TextSeparator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextSeparatorProperty = DependencyProperty.Register("TextSeparator", typeof(string), typeof(MultiSelectionComboBox), new PropertyMetadata(", "));

        // Using a DependencyProperty as the backing store for DisabledPopupOverlayConent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisabledPopupOverlayContentProperty =
            DependencyProperty.Register("DisabledPopupOverlayContent", typeof(object), typeof(MultiSelectionComboBox), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for DisabledPopupOverlayConentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisabledPopupOverlayContentTemplateProperty =
            DependencyProperty.Register("DisabledPopupOverlayContentTemplate", typeof(DataTemplate), typeof(MultiSelectionComboBox), new PropertyMetadata(null));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }


        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectionComboBox multiSelectionComboBox)
            {
                if ((bool)e.NewValue)
                {
                    multiSelectionComboBox.RaiseEvent(new RoutedEventArgs(DropDownOpenedEvent));

                    multiSelectionComboBox.Focus();

                    Mouse.Capture(multiSelectionComboBox, CaptureMode.SubTree);

                    multiSelectionComboBox.Dispatcher.BeginInvoke(
                       DispatcherPriority.Send,
                       (DispatcherOperationCallback)delegate (object arg)
                       {
                           MultiSelectionComboBox mscb = (MultiSelectionComboBox)arg;

                           var item = multiSelectionComboBox.SelectedItem ?? (mscb.HasItems ? mscb.Items[0] : null);
                           if (item != null)
                           {
                               var listBoxItem = mscb.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                               listBoxItem?.Focus();
                               ControlzEx.KeyboardNavigationEx.Focus(listBoxItem);
                           }

                           return null;
                       }, multiSelectionComboBox);
                }
                else
                {
                    multiSelectionComboBox.RaiseEvent(new RoutedEventArgs(DropDownClosedEvent));
                    if (Mouse.Captured == multiSelectionComboBox)
                    {
                        Mouse.Capture(null);
                    }
                }
            }
        }

        /// <summary>Identifies the <see cref="DropDownOpened"/> routed event.</summary>
        public static readonly RoutedEvent DropDownOpenedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(DropDownOpened),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(EventHandler<EventArgs>),
                                                                        typeof(MultiSelectionComboBox));

        /// <summary>Identifies the <see cref="DropDownClosed"/> routed event.</summary>
        public static readonly RoutedEvent DropDownClosedEvent = EventManager.RegisterRoutedEvent(
                                                                nameof(DropDownClosed),
                                                                RoutingStrategy.Bubble,
                                                                typeof(EventHandler<EventArgs>),
                                                                typeof(MultiSelectionComboBox));

        /// <summary>
        ///     Occurs when the DropDown is closed.
        /// </summary>
        public event EventHandler<EventArgs> DropDownClosed
        {
            add { AddHandler(DropDownClosedEvent, value); }
            remove { RemoveHandler(DropDownClosedEvent, value); }
        }

        /// <summary>
        ///     Occurs when the DropDown is opened.
        /// </summary>
        public event EventHandler<EventArgs> DropDownOpened
        {
            add { AddHandler(DropDownOpenedEvent, value); }
            remove { RemoveHandler(DropDownOpenedEvent, value); }
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            MultiSelectionComboBox multiSelectionComboBox = (MultiSelectionComboBox)sender;

            // If we (or one of our children) are clicked, claim the focus (don't steal focus if our context menu is clicked)
            if ((!multiSelectionComboBox.ContextMenu?.IsOpen ?? true) && !multiSelectionComboBox.IsKeyboardFocusWithin)
            {
                multiSelectionComboBox.Focus();
            }

            e.Handled = true;   // Always handle so that parents won't take focus away

            // Note: This half should be moved into OnMouseDownOutsideCapturedElement
            // When we have capture, all clicks off the popup will have the combobox as
            // the OriginalSource.  So when the original source is the combobox, that
            // means the click was off the popup and we should dismiss.
            if (Mouse.Captured == multiSelectionComboBox && e.OriginalSource == multiSelectionComboBox)
            {
                // multiSelectionComboBox.Close();
            }
        }

        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Indicates if the text is userdefined
        /// </summary>
        public bool HasCustomText
        {
            get { return (bool)GetValue(HasCustomTextProperty); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectionComboBox multiSelectionComboBox)
            {
                multiSelectionComboBox.UpdateEditableText();
            }
        }


        /// <summary>
        /// Updates the Text of the editable Textbox.
        /// Sets the custom Text if any otherwise the concatenated string.
        /// </summary>
        public void UpdateEditableText()
        {
            if (PART_EditableTextBox is null)
                return;

            isUpdatingText = true;
            if (string.IsNullOrEmpty(Text) && SelectionMode != SelectionMode.Single)
            {
                var items = ((IEnumerable<object>)SelectedItems).OrderBy(o => Items.IndexOf(o));
                PART_EditableTextBox.SetCurrentValue(TextBox.TextProperty, string.Join(TextSeparator, items));
                SetCurrentValue(HasCustomTextProperty, false);
            }
            else if ((string.IsNullOrEmpty(Text) && SelectionMode == SelectionMode.Single))
            {
                PART_EditableTextBox.SetCurrentValue(TextBox.TextProperty, SelectedItem);
            }
            else
            {
                PART_EditableTextBox.SetCurrentValue(TextBox.TextProperty, Text);
                SetCurrentValue(HasCustomTextProperty, true);
            }
            isUpdatingText = false;
        }


        public string TextSeparator
        {
            get { return (string)GetValue(TextSeparatorProperty); }
            set { SetValue(TextSeparatorProperty, value); }
        }


        public object DisabledPopupOverlayContent
        {
            get { return (object)GetValue(DisabledPopupOverlayContentProperty); }
            set { SetValue(DisabledPopupOverlayContentProperty, value); }
        }

        public DataTemplate DisabledPopupOverlayContentTemplate
        {
            get { return (DataTemplate)GetValue(DisabledPopupOverlayContentTemplateProperty); }
            set { SetValue(DisabledPopupOverlayContentTemplateProperty, value); }
        }

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
                e.CanExecute = multiSelectionComboBox.Text != null || multiSelectionComboBox.SelectedItems.Count > 0;
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

        #region DataTemplates

        // Using a DependencyProperty as the backing store for SeletedItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsTemplateProperty =
            DependencyProperty.Register(nameof(SelectedItemsTemplate), typeof(DataTemplate), typeof(MultiSelectionComboBox), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Selector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsTemplateSelectorProperty =
            DependencyProperty.Register(nameof(SelectedItemsTemplateSelector), typeof(DataTemplateSelector), typeof(MultiSelectionComboBox), new PropertyMetadata(null));
        
        
        public DataTemplate SelectedItemsTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemsTemplateProperty); }
            set { SetValue(SelectedItemsTemplateProperty, value); }
        }


        public DataTemplateSelector SelectedItemsTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectedItemsTemplateSelectorProperty); }
            set { SetValue(SelectedItemsTemplateSelectorProperty, value); }
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_EditableTextBox = GetTemplateChild(nameof(PART_EditableTextBox)) as TextBox;
            PART_EditableTextBox.TextChanged += PART_EditableTextBox_TextChanged;

            UpdateEditableText();

            PART_Popup = GetTemplateChild(nameof(PART_Popup)) as Popup;
            PART_PopupItemsPresenter = GetTemplateChild(nameof(PART_PopupItemsPresenter)) as ItemsPresenter;

            CommandBindings.Add(new CommandBinding(ClearContentCommand, ExecutedClearContentCommand, CanExecuteClearContentCommand));
            CommandBindings.Add(new CommandBinding(RemoveItemCommand, RemoveItemCommand_Executed, RemoveItemCommand_CanExecute));
        }

        bool isUpdatingText = false;
        private void PART_EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isUpdatingText)
            {
                var text = PART_EditableTextBox?.Text;

                if (text == string.Join(TextSeparator, (IEnumerable<object>)SelectedItems))
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


        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            UpdateEditableText();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Ignore the first mouse button up if we haven't gone over the popup yet
            // And ignore all mouse ups over the items host.
            if (!PART_Popup.IsMouseOver)
            {
                if (IsDropDownOpen)
                {
                    Close();
                    e.Handled = true;
                }
            }

            base.OnMouseLeftButtonUp(e);
        }

        private void Close()
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            MultiSelectionComboBox multiSelectionComboBox = (MultiSelectionComboBox)sender;

            // ISSUE (jevansa) -- task 22022:
            //        We need a general mechanism to do this, or at the very least we should
            //        share it amongst the controls which need it (Popup, MenuBase, ComboBox).
            if (Mouse.Captured != multiSelectionComboBox)
            {
                if (e.OriginalSource == multiSelectionComboBox)
                {
                    // If capture is null or it's not below the combobox, close.
                    // More workaround for task 22022 -- check if it's a descendant (following Logical links too)
                    if (Mouse.Captured == null || !(Mouse.Captured as DependencyObject).IsDescendantOf(multiSelectionComboBox))
                    {
                        multiSelectionComboBox.Close();
                    }
                }
                else
                {
                    if ((e.OriginalSource as DependencyObject).IsDescendantOf(multiSelectionComboBox))
                    {
                        // Take capture if one of our children gave up capture (by closing their drop down)
                        if (multiSelectionComboBox.IsDropDownOpen && Mouse.Captured == null)
                        {
                            Mouse.Capture(multiSelectionComboBox, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        multiSelectionComboBox.Close();
                    }
                }
                e.Handled = true;
            }
        }
        #endregion
    }
}
