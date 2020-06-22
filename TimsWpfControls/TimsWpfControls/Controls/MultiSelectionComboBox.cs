using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
                    multiSelectionComboBox.Dispatcher.BeginInvoke(
                        new Action(() => 
                        {
                            var item = multiSelectionComboBox.SelectedItem ?? (multiSelectionComboBox.HasItems ? multiSelectionComboBox.Items[0] : null);
                            if (item != null)
                            {
                                var listBoxItem = multiSelectionComboBox.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                                listBoxItem.Focus();
                                ControlzEx.KeyboardNavigationEx.Focus(listBoxItem);
                            }
                        }), System.Windows.Threading.DispatcherPriority.Send);
                }
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


        private void UpdateEditableText()
        {
            if (PART_EditableTextBox is null)
                return;

            isUpdatingText = true;
            if (string.IsNullOrEmpty(Text))
            {
                PART_EditableTextBox.SetCurrentValue(TextBox.TextProperty, string.Join(TextSeparator, (IEnumerable<object>)SelectedItems));
                SetCurrentValue(HasCustomTextProperty, false);
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
            PART_Popup.LostFocus += PART_Popup_LostFocus;

            CommandBindings.Add(new CommandBinding(ClearContentCommand, ExecutedClearContentCommand, CanExecuteClearContentCommand));
            CommandBindings.Add(new CommandBinding(RemoveItemCommand, RemoveItemCommand_Executed, RemoveItemCommand_CanExecute));
        }

        private void PART_Popup_LostFocus(object sender, RoutedEventArgs e)
        {
           if (!PART_Popup.IsKeyboardFocusWithin)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
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

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            SetCurrentValue(IsDropDownOpenProperty, false);
        }

        #endregion
    }
}
