﻿using ControlzEx;
using MahApps.Metro.Controls;
using MahApps.Metro.ValueBoxes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using TimsWpfControls.Helper;

namespace TimsWpfControls
{
    [TemplatePart(Name = nameof(PART_PopupListBox), Type = typeof(ListBox))]
    [TemplatePart(Name = nameof(PART_Popup), Type = typeof(Popup))]
    [TemplatePart(Name = nameof(PART_SelectedItemsPresenter), Type = typeof(ListBox))]
    [StyleTypedProperty(Property = nameof(SelectedItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    public class MultiSelectionComboBox : ComboBox
    {
        #region Constructors

        static MultiSelectionComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(typeof(MultiSelectionComboBox)));
            TextProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnTextChanged));
            CommandManager.RegisterClassCommandBinding(typeof(MultiSelectionComboBox), new CommandBinding(ClearContentCommand, ExecutedClearContentCommand, CanExecuteClearContentCommand));
            CommandManager.RegisterClassCommandBinding(typeof(MultiSelectionComboBox), new CommandBinding(RemoveItemCommand, RemoveItemCommand_Executed, RemoveItemCommand_CanExecute));
        }

        public MultiSelectionComboBox() : base()
        {
            var selectedItemsImpl = new ObservableCollection<object>();
            SetValue(SelectedItemsPropertyKey, selectedItemsImpl);

            selectedItemsImpl.CollectionChanged += SelectedItemsImpl_CollectionChanged;
        }


        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Members
        // 
        //-------------------------------------------------------------------

        #region private Members

        private Popup PART_Popup;
        private ListBox PART_PopupListBox;
        private TextBox PART_EditableTextBox;
        private ListBox PART_SelectedItemsPresenter;

        private bool isUserdefinedTextInputPending;
        private bool shouldDoTextReset; // Defines if the Text should be reset after selecting items from string
        private bool shouldAddItems; // Defines if the MSCB should add new items from text input. Don't set this to true while input is pending. We cannot know how long the user needs for typing.
        private bool IsSyncingSelectedItems; // true if syncing in one or the other direction already running
        private DispatcherTimer _updateSelectedItemsFromTextTimer;

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Properties
        // 
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>Identifies the <see cref="SelectionMode"/> dependency property.</summary>
        public static readonly DependencyProperty SelectionModeProperty
            = DependencyProperty.Register(nameof(SelectionMode),
                                          typeof(SelectionMode),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(SelectionMode.Single),
                                          IsValidSelectionMode);

        private static bool IsValidSelectionMode(object o)
        {
            SelectionMode value = (SelectionMode)o;
            return value == SelectionMode.Single
                   || value == SelectionMode.Multiple
                   || value == SelectionMode.Extended;
        }

        /// <summary>
        ///     Indicates the selection behavior for the ListBox.
        /// </summary>
        public SelectionMode SelectionMode
        {
            get => (SelectionMode)this.GetValue(SelectionModeProperty);
            set => this.SetValue(SelectionModeProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItem"/> dependency property.</summary>
        public static new readonly DependencyProperty SelectedItemProperty =
             DependencyProperty.Register(
                 nameof(SelectedItem),
                 typeof(object),
                 typeof(MultiSelectionComboBox),
                 new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or Sets the selectedItem
        /// </summary>
        public new object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedIndex"/> dependency property.</summary>
        public static new readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                nameof(SelectedIndex), typeof(int), typeof(MultiSelectionComboBox),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or Sets the SelectedIndex
        /// </summary>
        public new int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedValue"/> dependency property.</summary>
        public static new readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue),
                typeof(object),
                typeof(MultiSelectionComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or Sets the SelectedValue
        /// </summary>
        public new object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedItems"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey SelectedItemsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SelectedItems),
                                                  typeof(IList),
                                                  typeof(MultiSelectionComboBox),
                                                  new PropertyMetadata((IList)null));

        /// <summary>Identifies the <see cref="SelectedItems"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// The currently selected items.
        /// </summary>
        [Bindable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList SelectedItems
        {
            get => (IList)this.GetValue(SelectedItemsProperty);
            protected set => this.SetValue(SelectedItemsPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="DisplaySelectedItems"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey DisplaySelectedItemsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(DisplaySelectedItems),
                                                  typeof(IEnumerable),
                                                  typeof(MultiSelectionComboBox),
                                                  new PropertyMetadata((IEnumerable)null));

        /// <summary>Identifies the <see cref="DisplaySelectedItems"/> dependency property.</summary>
        public static readonly DependencyProperty DisplaySelectedItemsProperty = DisplaySelectedItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="SelectedItems"/> in the specified order which was set via <see cref="OrderSelectedItemsBy"/>
        /// </summary>
        public IEnumerable DisplaySelectedItems
        {
            get => (IEnumerable)this.GetValue(DisplaySelectedItemsProperty);
            protected set => this.SetValue(DisplaySelectedItemsPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="OrderSelectedItemsBy"/> dependency property.</summary>
        public static readonly DependencyProperty OrderSelectedItemsByProperty
            = DependencyProperty.Register(nameof(OrderSelectedItemsBy),
                                          typeof(SelectedItemsOrderType),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(SelectedItemsOrderType.SelectedOrder, OnOrderSelectedItemsByChanged));

        /// <summary>
        /// Gets or sets how the <see cref="SelectedItems"/> should be sorted
        /// </summary>
        public SelectedItemsOrderType OrderSelectedItemsBy
        {
            get => (SelectedItemsOrderType)this.GetValue(OrderSelectedItemsByProperty);
            set => this.SetValue(OrderSelectedItemsByProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemContainerStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemContainerStyleProperty
            = DependencyProperty.Register(nameof(SelectedItemContainerStyle),
                                          typeof(Style),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Style"/> for the <see cref="SelectedItems"/>
        /// </summary>
        public Style SelectedItemContainerStyle
        {
            get => (Style)this.GetValue(SelectedItemContainerStyleProperty);
            set => this.SetValue(SelectedItemContainerStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemContainerStyleSelector"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemContainerStyleSelectorProperty
            = DependencyProperty.Register(nameof(SelectedItemContainerStyleSelector),
                                          typeof(StyleSelector),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="StyleSelector"/> for the <see cref="SelectedItemContainerStyle"/>
        /// </summary>
        public StyleSelector SelectedItemContainerStyleSelector
        {
            get => (StyleSelector)this.GetValue(SelectedItemContainerStyleSelectorProperty);
            set => this.SetValue(SelectedItemContainerStyleSelectorProperty, value);
        }

        /// <summary>Identifies the <see cref="Separator"/> dependency property.</summary>
        public static readonly DependencyProperty SeparatorProperty
            = DependencyProperty.Register(nameof(Separator),
                                          typeof(string),
                                          typeof(MultiSelectionComboBox),
                                          new FrameworkPropertyMetadata(null, UpdateText));

        /// <summary>
        /// Gets or Sets the Separator which will be used if the ComboBox is editable.
        /// </summary>
        public string Separator
        {
            get => (string)this.GetValue(SeparatorProperty);
            set => this.SetValue(SeparatorProperty, value);
        }

        /// <summary>Identifies the <see cref="HasCustomText"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey HasCustomTextPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(HasCustomText),
                                                  typeof(bool),
                                                  typeof(MultiSelectionComboBox),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the <see cref="HasCustomText"/> dependency property.</summary>
        public static readonly DependencyProperty HasCustomTextProperty = HasCustomTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates if the text is user defined
        /// </summary>
        public bool HasCustomText
        {
            get => (bool)this.GetValue(HasCustomTextProperty);
            protected set => this.SetValue(HasCustomTextPropertyKey, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TextWrapping"/> dependency property.</summary>
        public static readonly DependencyProperty TextWrappingProperty
            = TextBlock.TextWrappingProperty.AddOwner(typeof(MultiSelectionComboBox),
                                                      new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The TextWrapping property controls whether or not text wraps
        /// when it reaches the flow edge of its containing block box.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)this.GetValue(TextWrappingProperty);
            set => this.SetValue(TextWrappingProperty, value);
        }

        /// <summary>Identifies the <see cref="AcceptsReturn"/> dependency property.</summary>
        public static readonly DependencyProperty AcceptsReturnProperty
            = TextBoxBase.AcceptsReturnProperty.AddOwner(typeof(MultiSelectionComboBox));

        /// <summary>
        /// The TextWrapping property controls whether or not text wraps
        /// when it reaches the flow edge of its containing block box.
        /// </summary>
        public bool AcceptsReturn
        {
            get => (bool)this.GetValue(AcceptsReturnProperty);
            set => this.SetValue(AcceptsReturnProperty, value);
        }

        /// <summary>Identifies the <see cref="ObjectToStringComparer"/> dependency property.</summary>
        public static readonly DependencyProperty ObjectToStringComparerProperty
            = DependencyProperty.Register(nameof(ObjectToStringComparer),
                                          typeof(ICompareObjectToString),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a function that is used to check if the entered Text is an object that should be selected.
        /// </summary>
        public ICompareObjectToString ObjectToStringComparer
        {
            get => (ICompareObjectToString)this.GetValue(ObjectToStringComparerProperty);
            set => this.SetValue(ObjectToStringComparerProperty, value);
        }

        /// <summary>Identifies the <see cref="EditableTextStringComparision"/> dependency property.</summary>
        public static readonly DependencyProperty EditableTextStringComparisionProperty
            = DependencyProperty.Register(nameof(EditableTextStringComparision),
                                          typeof(StringComparison),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(StringComparison.Ordinal));

        /// <summary>
        ///  Gets or Sets the <see cref="StringComparison"/> that is used to check if the entered <see cref="ComboBox.Text"/> matches to the <see cref="SelectedItems"/>
        /// </summary>
        public StringComparison EditableTextStringComparision
        {
            get => (StringComparison)this.GetValue(EditableTextStringComparisionProperty);
            set => this.SetValue(EditableTextStringComparisionProperty, value);
        }

        /// <summary>Identifies the <see cref="StringToObjectParser"/> dependency property.</summary>
        public static readonly DependencyProperty StringToObjectParserProperty
            = DependencyProperty.Register(nameof(StringToObjectParser),
                                          typeof(IParseStringToObject),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a parser-class that implements <see cref="IParseStringToObject"/> 
        /// </summary>
        public IParseStringToObject StringToObjectParser
        {
            get => (IParseStringToObject)this.GetValue(StringToObjectParserProperty);
            set => this.SetValue(StringToObjectParserProperty, value);
        }

        /// <summary>Identifies the <see cref="DisabledPopupOverlayContent"/> dependency property.</summary>
        public static readonly DependencyProperty DisabledPopupOverlayContentProperty
            = DependencyProperty.Register(nameof(DisabledPopupOverlayContent),
                                          typeof(object),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the DisabledPopupOverlayContent
        /// </summary>
        public object DisabledPopupOverlayContent
        {
            get => (object)this.GetValue(DisabledPopupOverlayContentProperty);
            set => this.SetValue(DisabledPopupOverlayContentProperty, value);
        }

        /// <summary>Identifies the <see cref="DisabledPopupOverlayContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty DisabledPopupOverlayContentTemplateProperty
            = DependencyProperty.Register(nameof(DisabledPopupOverlayContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the DisabledPopupOverlayContentTemplate
        /// </summary>
        public DataTemplate DisabledPopupOverlayContentTemplate
        {
            get => (DataTemplate)this.GetValue(DisabledPopupOverlayContentTemplateProperty);
            set => this.SetValue(DisabledPopupOverlayContentTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemTemplateProperty
            = DependencyProperty.Register(nameof(SelectedItemTemplate),
                                          typeof(DataTemplate),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the SelectedItemTemplate
        /// </summary>
        public DataTemplate SelectedItemTemplate
        {
            get => (DataTemplate)this.GetValue(SelectedItemTemplateProperty);
            set => this.SetValue(SelectedItemTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemTemplateSelectorProperty
            = DependencyProperty.Register(nameof(SelectedItemTemplateSelector),
                                          typeof(DataTemplateSelector),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the SelectedItemTemplateSelector
        /// </summary>
        public DataTemplateSelector SelectedItemTemplateSelector
        {
            get => (DataTemplateSelector)this.GetValue(SelectedItemTemplateSelectorProperty);
            set => this.SetValue(SelectedItemTemplateSelectorProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemStringFormatProperty
            = DependencyProperty.Register(nameof(SelectedItemStringFormat),
                                          typeof(string),
                                          typeof(MultiSelectionComboBox),
                                          new FrameworkPropertyMetadata(null, UpdateText));

        /// <summary>
        /// Gets or Sets the string format for the selected items
        /// </summary>
        public string SelectedItemStringFormat
        {
            get => (string)this.GetValue(SelectedItemStringFormatProperty);
            set => this.SetValue(SelectedItemStringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty
            = DependencyProperty.Register(nameof(VerticalScrollBarVisibility),
                                          typeof(ScrollBarVisibility),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Gets or Sets if the vertical scrollbar is visible
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)this.GetValue(VerticalScrollBarVisibilityProperty);
            set => this.SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        /// <summary>Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty
            = DependencyProperty.Register(nameof(HorizontalScrollBarVisibility),
                                          typeof(ScrollBarVisibility),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Gets or Sets if the horizontal scrollbar is visible
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)this.GetValue(HorizontalScrollBarVisibilityProperty);
            set => this.SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemsPanelTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemsPanelTemplateProperty
            = DependencyProperty.Register(nameof(SelectedItemsPanelTemplate),
                                          typeof(ItemsPanelTemplate),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="ItemsPanelTemplate"/> for the selected items.
        /// </summary>
        public ItemsPanelTemplate SelectedItemsPanelTemplate
        {
            get => (ItemsPanelTemplate)this.GetValue(SelectedItemsPanelTemplateProperty);
            set => this.SetValue(SelectedItemsPanelTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectItemsFromTextInputDelay"/> dependency property.</summary>
        public static readonly DependencyProperty SelectItemsFromTextInputDelayProperty
            = DependencyProperty.Register(nameof(SelectItemsFromTextInputDelay),
                                          typeof(int),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(-1));

        /// <summary>
        /// Gets or Sets the delay in milliseconds to wait before the selection is updated during text input.
        /// If this value is -1 the selection will not be updated during text input. 
        /// Note: You also need to set <see cref="ObjectToStringComparer"/> to get this to work. 
        /// </summary>
        public int SelectItemsFromTextInputDelay
        {
            get => (int)this.GetValue(SelectItemsFromTextInputDelayProperty);
            set => this.SetValue(SelectItemsFromTextInputDelayProperty, value);
        }


        /// <summary>Identifies the <see cref="InterceptKeyboardSelection"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptKeyboardSelectionProperty
            = DependencyProperty.Register(nameof(InterceptKeyboardSelection),
                                          typeof(bool),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or Sets if the user can select items from the keyborad, e.g. with the ▲ ▼ Keys. 
        /// This property is only applied when the <see cref="SelectionMode"/> is <see cref="SelectionMode.Single"/>
        /// </summary>
        public bool InterceptKeyboardSelection
        {
            get => (bool)this.GetValue(InterceptKeyboardSelectionProperty);
            set => this.SetValue(InterceptKeyboardSelectionProperty, value);
        }

        /// <summary>Identifies the <see cref="InterceptMouseWheelSelection"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptMouseWheelSelectionProperty
            = DependencyProperty.Register(nameof(InterceptMouseWheelSelection),
                                          typeof(bool),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or Sets if the user can select items by mouse wheel. 
        /// This property is only applied when the <see cref="SelectionMode"/> is <see cref="SelectionMode.Single"/>
        /// </summary>
        public bool InterceptMouseWheelSelection
        {
            get => (bool)this.GetValue(InterceptMouseWheelSelectionProperty);
            set => this.SetValue(InterceptMouseWheelSelectionProperty, value);
        }

        /// <summary>
        /// Resets the custom Text to the selected Items text 
        /// </summary>
        public void ResetEditableText()
        {
            var oldSelectionStart = this.PART_EditableTextBox.SelectionStart;
            var oldSelectionLength = this.PART_EditableTextBox.SelectionLength;

            this.SetValue(HasCustomTextPropertyKey, false);
            this.UpdateEditableText();

            this.PART_EditableTextBox.SelectionStart = oldSelectionStart;
            this.PART_EditableTextBox.SelectionLength = oldSelectionLength;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the Text of the editable Textbox.
        /// Sets the custom Text if any otherwise the concatenated string.
        /// </summary>
        private void UpdateEditableText(bool forceUpdate = false)
        {
            if (this.PART_EditableTextBox is null || (PART_EditableTextBox.IsKeyboardFocused && !forceUpdate))
            {
                return;
            }

            var oldSelectionStart = PART_EditableTextBox.SelectionStart;
            var oldSelectionLength = PART_EditableTextBox.SelectionLength;
            var oldTextLenth = PART_EditableTextBox.Text.Length;

            var selectedItemsText = this.GetSelectedItemsText();

            if (!this.HasCustomText)
            {
                this.SetCurrentValue(TextProperty, selectedItemsText);
            }

            this.UpdateHasCustomText(selectedItemsText);

            if (oldSelectionLength == oldTextLenth) // We had all Text selected, so we select all again
            {
                PART_EditableTextBox.SelectionStart = 0;
                PART_EditableTextBox.SelectionLength = PART_EditableTextBox.Text.Length;
            }
            else if (oldSelectionStart == oldTextLenth) // we had the cursor at the last position, so we move the cursor to the end again
            {
                PART_EditableTextBox.SelectionStart = PART_EditableTextBox.Text.Length;
            }
            else // we retore the old selection
            {
                PART_EditableTextBox.SelectionStart = oldSelectionStart;
                PART_EditableTextBox.SelectionLength = oldSelectionLength;
            }
        }

        private void UpdateDisplaySelectedItems()
        {
            this.UpdateDisplaySelectedItems(this.OrderSelectedItemsBy);
        }

        public string GetSelectedItemsText()
        {
            switch (this.SelectionMode)
            {
                case SelectionMode.Single:
                    if (this.ReadLocalValue(DisplayMemberPathProperty) != DependencyProperty.UnsetValue
                        || this.ReadLocalValue(SelectedItemStringFormatProperty) != DependencyProperty.UnsetValue)
                    {
                        return BindingHelper.Eval(this.SelectedItem, this.DisplayMemberPath ?? string.Empty, this.SelectedItemStringFormat)?.ToString();
                    }
                    else
                    {
                        return this.SelectedItem?.ToString();
                    }

                case SelectionMode.Multiple:
                case SelectionMode.Extended:
                    IEnumerable<object> values;

                    if (this.ReadLocalValue(DisplayMemberPathProperty) != DependencyProperty.UnsetValue
                        || this.ReadLocalValue(SelectedItemStringFormatProperty) != DependencyProperty.UnsetValue)
                    {
                        values = ((IEnumerable<object>)this.DisplaySelectedItems)?.Select(o => BindingHelper.Eval(o, this.DisplayMemberPath ?? string.Empty, this.SelectedItemStringFormat));
                    }
                    else
                    {
                        values = (IEnumerable<object>)this.DisplaySelectedItems;
                    }

                    return values is null ? null : string.Join(this.Separator ?? string.Empty, values);

                default:
                    return null;
            }
        }

        private void UpdateHasCustomText(string selectedItemsText)
        {
            // if the parameter was null lets get the text on our own.
            selectedItemsText ??= this.GetSelectedItemsText();

            this.HasCustomText = !((string.IsNullOrEmpty(selectedItemsText) && string.IsNullOrEmpty(this.Text))
                                   || string.Equals(this.Text, selectedItemsText, this.EditableTextStringComparision));
        }

        private void UpdateDisplaySelectedItems(SelectedItemsOrderType selectedItemsOrderType)
        {
            var displaySelectedItems = selectedItemsOrderType switch
            {
                SelectedItemsOrderType.SelectedOrder => this.SelectedItems,
                SelectedItemsOrderType.ItemsSourceOrder => ((IEnumerable<object>)this.SelectedItems).OrderBy(o => this.Items.IndexOf(o)),
                _ => this.DisplaySelectedItems
            };

            this.SetValue(DisplaySelectedItemsPropertyKey, displaySelectedItems);
        }

        private void SelectItemsFromText(int millisecondsToWait)
        {
            if (!this.isUserdefinedTextInputPending)
            {
                return;
            }

            // We want to do a text reset or add items only if we don't need to wait for more input. 
            shouldDoTextReset = millisecondsToWait == 0;
            shouldAddItems = millisecondsToWait == 0;

            if (this._updateSelectedItemsFromTextTimer is null)
            {
                this._updateSelectedItemsFromTextTimer = new DispatcherTimer(DispatcherPriority.Background);
                this._updateSelectedItemsFromTextTimer.Tick += this.UpdateSelectedItemsFromTextTimer_Tick;
            }

            if (this._updateSelectedItemsFromTextTimer.IsEnabled)
            {
                this._updateSelectedItemsFromTextTimer.Stop();
            }

            if (!(this.ObjectToStringComparer is null) && (!string.IsNullOrEmpty(this.Separator) || this.SelectionMode == SelectionMode.Single))
            {
                this._updateSelectedItemsFromTextTimer.Interval = TimeSpan.FromMilliseconds(millisecondsToWait > 0 ? millisecondsToWait : 0);
                this._updateSelectedItemsFromTextTimer.Start();
            }
        }


        private void UpdateSelectedItemsFromTextTimer_Tick(object sender, EventArgs e)
        {
            this._updateSelectedItemsFromTextTimer.Stop();



            // We clear the selection if there is no text available. 
            if (string.IsNullOrEmpty(this.Text))
            {
                switch (this.SelectionMode)
                {
                    case SelectionMode.Single:
                        this.SetCurrentValue(SelectedItemProperty, null);
                        break;
                    case SelectionMode.Multiple:
                    case SelectionMode.Extended:
                        this.SelectedItems.Clear();
                        break;
                    default:
                        throw new NotSupportedException("Unknown SelectionMode");
                }

                return;
            }

            bool foundItem;

            switch (this.SelectionMode)
            {
                case SelectionMode.Single:
                    foundItem = false;

                    if (ObjectToStringComparer.CheckIfStringMatchesObject(Text, SelectedItem, EditableTextStringComparision, SelectedItemStringFormat))
                    {
                        foundItem = true;
                    }
                    else
                    {
                        for (int i = 0; i < Items.Count; i++)
                        {
                            if (ObjectToStringComparer.CheckIfStringMatchesObject(Text, Items[i], EditableTextStringComparision, SelectedItemStringFormat))
                            {
                                SetCurrentValue(SelectedItemProperty, Items[i]);
                                foundItem = true;
                                break;
                            }
                        }
                    }

                    if (!foundItem)
                    {
                        // We try to add a new item. If we were able to do so we need to update the text as it may differ. 
                        if (shouldAddItems && TryAddObjectFromString(Text, out object result))
                        {
                            SelectedItem = result;
                        }
                        else
                        {
                            SetCurrentValue(SelectedItemProperty, null);
                            shouldDoTextReset = false; // We did not find the needed item so we should not do the text reset.
                        }
                    }
                    break;

                case SelectionMode.Multiple:
                case SelectionMode.Extended:

                    var strings = Text.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

                    int k = 0; // our counter index to insert into the selection

                    for (int i = 0; i < strings.Length; i++)
                    {
                        foundItem = false;

                        // First we check if we have this item already selected
                        for (int j = k; j < SelectedItems.Count; j++)
                        {
                            if (ObjectToStringComparer.CheckIfStringMatchesObject(strings[i], SelectedItems[j], EditableTextStringComparision, SelectedItemStringFormat))
                            {
                                foundItem = true;

                                // We move the item to the right index
                                if (j > k)
                                {
                                    var itemToMove = SelectedItems[j];
                                    SelectedItems.RemoveAt(j);
                                    SelectedItems.Insert(k++, itemToMove);
                                }
                                else
                                {
                                    k++;
                                }
                            }
                        }

                        // if we have the item not already selected we will check if we have it in our items collection
                        if (!foundItem)
                        {
                            for (int j = 0; j < Items.Count; j++)
                            {
                                if (ObjectToStringComparer.CheckIfStringMatchesObject(strings[i], Items[j], EditableTextStringComparision, SelectedItemStringFormat))
                                {
                                    SelectedItems.Insert(k++, Items[j]);
                                    foundItem = true;
                                }
                            }
                        }

                        // If we still have no item found we try to add a new item
                        if (!foundItem)
                        {
                            if (shouldAddItems && TryAddObjectFromString(strings[i], out object result))
                            {
                                SelectedItems.Insert(k++, result);
                            }
                            else
                            {
                                shouldDoTextReset = false;
                            }
                        }
                    }

                    // We may have more items in our List than needed, so we need to remove the extended
                    for (int j = SelectedItems.Count - 1; j >= k; j--)
                    {
                        SelectedItems.RemoveAt(j);
                    }
                    break;

                default:
                    throw new NotSupportedException("Unknown SelectionMode");
            }

            // First we need to check if the string matches completely to the selected items. Therefore we need to display the items in the selected order first
            this.UpdateDisplaySelectedItems(SelectedItemsOrderType.SelectedOrder);
            this.UpdateHasCustomText(null);

            // If the items should be ordered differntly than above we need to reoder them accordingly.
            if (this.OrderSelectedItemsBy != SelectedItemsOrderType.SelectedOrder)
            {
                this.UpdateDisplaySelectedItems();
            }

            // We do a text reset if all items were successfully found and we don't have to wait for more input.
            if (shouldDoTextReset)
            {
                var oldCaretPos = this.PART_EditableTextBox.CaretIndex;
                this.ResetEditableText();
                this.PART_EditableTextBox.CaretIndex = oldCaretPos;
            }

            // If we have the KeyboardFocus we need to update the text later in order to not inerrupt the user.
            // Therefore we connect this flag to the KeyboardFocus of the TextBox.
            this.isUserdefinedTextInputPending = PART_EditableTextBox.IsKeyboardFocused;
        }

        private bool TryAddObjectFromString(string input, out object result)
        {
            try
            {
                if (StringToObjectParser is null)
                {
                    result = null;
                    return false;
                }

                var elementType = BuiltInStringToObjectParser.Instance.GetElementType(ItemsSource);

                var foundItem = this.StringToObjectParser.TryCreateObjectFromString(input, out result, this.Language.GetEquivalentCulture(), this.SelectedItemStringFormat, elementType);

                var addingItemEventArgs = new AddingItemEventArgs(AddingItemEvent,
                                                                  this,
                                                                  input,
                                                                  result,
                                                                  foundItem,
                                                                  this.ReadLocalValue(ItemsSourceProperty) == DependencyProperty.UnsetValue ? this.ItemsSource as IList : this.ItemsSource as IList,
                                                                  elementType,
                                                                  this.SelectedItemStringFormat,
                                                                  this.Language.GetEquivalentCulture(),
                                                                  this.StringToObjectParser);

                this.RaiseEvent(addingItemEventArgs);

                if (addingItemEventArgs.Handled)
                {
                    addingItemEventArgs.Accepted = false;
                }

                // If the adding event was not handled and the item is markeed as accepted and we are allowed to modify the items list we can add the pared item
                if (addingItemEventArgs.Accepted && (!addingItemEventArgs.TargetList?.IsReadOnly ?? false))
                {
                    addingItemEventArgs.TargetList.Add(addingItemEventArgs.ParsedObject);

                    this.RaiseEvent(new AddedItemEventArgs(AddedItemEvent, this, addingItemEventArgs.ParsedObject, addingItemEventArgs.TargetList));
                }

                result = addingItemEventArgs.ParsedObject;
                return addingItemEventArgs.Accepted;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                result = null;
                return false;
            }
        }

        #endregion

        #region Commands

        // Clear Text Command
        public static RoutedUICommand ClearContentCommand { get; } = new RoutedUICommand("ClearContent", nameof(ClearContentCommand), typeof(MultiSelectionComboBox));

        private static void ExecutedClearContentCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MultiSelectionComboBox multiSelectionCombo)
            {
                if (multiSelectionCombo.HasCustomText)
                {
                    multiSelectionCombo.ResetEditableText();
                }
                else
                {
                    switch (multiSelectionCombo.SelectionMode)
                    {
                        case SelectionMode.Single:
                            multiSelectionCombo.SetCurrentValue(SelectedItemProperty, null);
                            break;
                        case SelectionMode.Multiple:
                        case SelectionMode.Extended:
                            multiSelectionCombo.SelectedItems.Clear();
                            break;
                        default:
                            throw new NotSupportedException("Unknown SelectionMode");
                    }
                }
            }
        }

        private static void CanExecuteClearContentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (sender is MultiSelectionComboBox multiSelectionComboBox)
            {
                e.CanExecute = !string.IsNullOrEmpty(multiSelectionComboBox.Text) || multiSelectionComboBox.SelectedItems.Count > 0;
            }
        }

        public static RoutedUICommand RemoveItemCommand { get; } = new RoutedUICommand("Remove item", nameof(RemoveItemCommand), typeof(MultiSelectionComboBox));

        private static void RemoveItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MultiSelectionComboBox multiSelectionCombo)
            {
                if (multiSelectionCombo.SelectionMode == SelectionMode.Single)
                {
                    multiSelectionCombo.SetCurrentValue(SelectedItemProperty, null);
                    return;
                }

                if (multiSelectionCombo.SelectedItems.Contains(e.Parameter))
                {
                    multiSelectionCombo.SelectedItems.Remove(e.Parameter);
                }
            }
        }

        private static void RemoveItemCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (sender is MultiSelectionComboBox)
            {
                e.CanExecute = e.Parameter != null;
            }
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Init SelectedItemsPresenter
            this.PART_SelectedItemsPresenter = this.GetTemplateChild(nameof(this.PART_SelectedItemsPresenter)) as ListBox;

            if (!(this.PART_SelectedItemsPresenter is null))
            {
                this.PART_SelectedItemsPresenter.MouseLeftButtonUp -= this.PART_SelectedItemsPresenter_MouseLeftButtonUp;
                this.PART_SelectedItemsPresenter.SelectionChanged -= this.PART_SelectedItemsPresenter_SelectionChanged;

                this.PART_SelectedItemsPresenter.MouseLeftButtonUp += this.PART_SelectedItemsPresenter_MouseLeftButtonUp;
                this.PART_SelectedItemsPresenter.SelectionChanged += this.PART_SelectedItemsPresenter_SelectionChanged;
            }
            else
            {
                throw new ArgumentNullException($"The template part \"{nameof(this.PART_SelectedItemsPresenter)}\" could not be found.");
            }

            // Init EditableTextBox
            this.PART_EditableTextBox = this.GetTemplateChild(nameof(this.PART_EditableTextBox)) as TextBox;

            if (!(this.PART_EditableTextBox is null))
            {
                this.PART_EditableTextBox.LostFocus -= this.PART_EditableTextBox_LostFocus;
                this.PART_EditableTextBox.LostFocus += this.PART_EditableTextBox_LostFocus;
            }
            else
            {
                throw new ArgumentNullException($"The template part \"{nameof(this.PART_EditableTextBox)}\" could not be found.");
            }

            // Init Popup
            this.PART_Popup = this.GetTemplateChild(nameof(this.PART_Popup)) as Popup;

            if (this.PART_Popup is null)
            {
                throw new ArgumentNullException($"The template part \"{nameof(this.PART_Popup)}\" could not be found.");
            }

            this.PART_PopupListBox = this.GetTemplateChild(nameof(this.PART_PopupListBox)) as ListBox;

            if (!(this.PART_PopupListBox is null) && this.PART_PopupListBox.SelectedItems is INotifyCollectionChanged selectedItemsCollection)
            {
                selectedItemsCollection.CollectionChanged -= PART_PopupListBox_SelectedItems_CollectionChanged;
                selectedItemsCollection.CollectionChanged += PART_PopupListBox_SelectedItems_CollectionChanged;
            }
            else
            {
                throw new ArgumentNullException($"The template part \"{nameof(this.PART_PopupListBox)}\" could not be found.");
            }

            // Do update the text 
            this.UpdateDisplaySelectedItems();
            this.UpdateEditableText(true);
        }

        private void PART_PopupListBox_SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SyncSelectedItems(PART_PopupListBox.SelectedItems, SelectedItems, e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            this.UpdateEditableText();
            this.UpdateDisplaySelectedItems();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (!this.IsLoaded)
            {
                this.Loaded += this.MultiSelectionComboBox_Loaded;
                return;
            }

            // If we have the ItemsSource set, we need to exit here. 
            if (((PART_PopupListBox?.Items as IList)?.IsReadOnly ?? false) || BindingOperations.IsDataBound(this, ItemsSourceProperty))
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        this.PART_PopupListBox.Items.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        this.PART_PopupListBox.Items.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    this.PART_PopupListBox.Items.Clear();
                    foreach (var item in this.Items)
                    {
                        this.PART_PopupListBox.Items.Add(item);
                    }

                    break;
                default:
                    throw new NotSupportedException("Unsupported NotifyCollectionChangedAction");
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // For now we only want to update our poition if the height changed. Else we will get a flickering in SharedGridColumns
            if (this.IsDropDownOpen && sizeInfo.HeightChanged && !(this.PART_Popup is null))
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                            (DispatcherOperationCallback)((object arg) =>
                                            {
                                                MultiSelectionComboBox mscb = (MultiSelectionComboBox)arg;
                                                mscb.PART_Popup.HorizontalOffset++;
                                                mscb.PART_Popup.HorizontalOffset--;

                                                return null;
                                            }), this);
            }
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);

            this.PART_PopupListBox.Focus();

            if (this.PART_PopupListBox.Items.Count == 0)
            {
                return;
            }

            MoveFocusToDropDown();

            this.SelectItemsFromText(0);
        }

        /// <summary>
        /// Sets the Keyboard focus to the dropdown
        /// </summary>
        private void MoveFocusToDropDown()
        {
            var index = this.PART_PopupListBox.SelectedIndex;
            if (index < 0)
            {
                index = 0;
            }

            Action action = () =>
            {
                ListBoxItem item = null;
                if (index >= 0)
                {
                    PART_PopupListBox.ScrollIntoView(PART_PopupListBox.Items[index]);
                    item = this.PART_PopupListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                }

                if (item != null)
                {
                    item.Focus();
                    KeyboardNavigationEx.Focus(item);
                    this.PART_PopupListBox.ScrollIntoView(item);
                }
                else
                {
                    this.PART_Popup.Focus();
                }
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, action);
        }

        /// <summary>
        /// Return true if the item is (or is eligible to be) its own ItemUI
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ListBoxItem);
        }

        /// <summary> Create or identify the element used to display the given item. </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListBoxItem();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (this.IsEditable && !this.IsDropDownOpen && !(this.PART_EditableTextBox is null) && !this.InterceptKeyboardSelection)
            {
                if (this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled && ScrollViewerHelper.GetIsHorizontalScrollWheelEnabled(this))
                {
                    if (e.Delta > 0)
                    {
                        this.PART_EditableTextBox.LineLeft();
                    }
                    else
                    {
                        this.PART_EditableTextBox.LineRight();
                    }
                }
                else
                {
                    if (e.Delta > 0)
                    {
                        this.PART_EditableTextBox.LineUp();
                    }
                    else
                    {
                        this.PART_EditableTextBox.LineDown();
                    }
                }
            }
            else if (!IsEditable && !this.IsDropDownOpen && !(this.PART_SelectedItemsPresenter is null) && !this.InterceptMouseWheelSelection)
            {
                var scrollViewer = PART_SelectedItemsPresenter.FindChild<ScrollViewer>();
                if (scrollViewer?.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled && ScrollViewerHelper.GetIsHorizontalScrollWheelEnabled(this))
                {
                    if (e.Delta > 0)
                    {
                        scrollViewer?.LineLeft();
                    }
                    else
                    {
                        scrollViewer?.LineRight();
                    }
                }
                else
                {
                    if (e.Delta > 0)
                    {
                        scrollViewer?.LineUp();
                    }
                    else
                    {
                        scrollViewer?.LineDown();
                    }
                }
            }
            // ListBox eats the selection so we need to handle this event here if we want to select the next item.
            else if (!this.IsDropDownOpen && this.InterceptMouseWheelSelection && this.SelectionMode == SelectionMode.Single)
            {
                if (e.Delta > 0 && this.PART_PopupListBox.SelectedIndex > 0)
                {
                    this.SelectPrev();
                }
                else if (e.Delta < 0 && this.PART_PopupListBox.SelectedIndex < this.PART_PopupListBox.Items.Count - 1)
                {
                    this.SelectNext();
                }
            }

            // The event is handled if the drop down is not open. 
            e.Handled = !this.IsDropDownOpen;
            base.OnPreviewMouseWheel(e);
        }

        /// <summary>
        ///     An event reporting a key was pressed
        /// </summary>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Only process preview key events if they going to our editable text box
            if (IsEditable && e.OriginalSource == PART_EditableTextBox)
            {
                KeyDownHandler(e);
            }
        }

        /// <summary>
        ///     An event reporting a key was pressed
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyDownHandler(e);
        }

        private void KeyDownHandler(KeyEventArgs e)
        {
            bool handled = false;
            Key key = e.Key;

            // We want to handle Alt key. Get the real key if it is Key.System.
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            // In Right to Left mode we switch Right and Left keys
            bool isRTL = (FlowDirection == FlowDirection.RightToLeft);

            switch (key)
            {
                case Key.Up:
                    handled = true;
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                    {
                        this.IsDropDownOpen = !this.IsDropDownOpen;
                    }
                    else
                    {
                        // When the drop down isn't open then focus is on the ComboBox
                        // and we can't use KeyboardNavigation.
                        if (IsDropDownOpen)
                        {
                            MoveFocusToDropDown();
                        }
                        else if (!IsDropDownOpen && InterceptKeyboardSelection && SelectionMode == SelectionMode.Single)
                        {
                            this.SelectPrev();
                        }
                    }

                    break;

                case Key.Down:
                    handled = true;
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                    {
                        this.IsDropDownOpen = !this.IsDropDownOpen;
                    }
                    else
                    {
                        // When the drop down isn't open then focus is on the ComboBox
                        // and we can't use KeyboardNavigation.
                        if (IsDropDownOpen)
                        {
                            MoveFocusToDropDown();
                        }
                        else if (!IsDropDownOpen && InterceptKeyboardSelection && SelectionMode == SelectionMode.Single)
                        {
                            this.SelectNext();
                        }
                    }

                    break;

                case Key.F4:
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == 0)
                    {
                        this.IsDropDownOpen = !this.IsDropDownOpen;
                        handled = true;
                    }
                    break;

                case Key.Escape:
                    base.OnKeyDown(e);
                    break;

                case Key.Enter:
                    if (IsDropDownOpen)
                    {
                        base.OnKeyDown(e);
                    }
                    break;

                case Key.Home:
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt && !IsEditable)
                    {
                        if (!IsDropDownOpen && InterceptKeyboardSelection && SelectionMode == SelectionMode.Single)
                        {
                            SelectFirst();
                        }
                        handled = true;
                    }
                    break;

                case Key.End:
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt && !IsEditable)
                    {
                        if (!IsDropDownOpen && InterceptKeyboardSelection && SelectionMode == SelectionMode.Single)
                        {
                            SelectLast();
                        }
                        handled = true;
                    }
                    break;

                case Key.Right:
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt && !IsEditable)
                    {
                        if (IsDropDownOpen)
                        {
                            MoveFocusToDropDown();
                        }
                        else
                        {
                            if (!isRTL)
                            {
                                SelectNext();
                            }
                            else if (!IsDropDownOpen && InterceptKeyboardSelection && SelectionMode == SelectionMode.Single)
                            {
                                // If it's RTL then Right should go backwards
                                SelectPrev();
                            }
                        }
                        handled = true;
                    }
                    break;

                case Key.Left:
                    if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt && !IsEditable)
                    {
                        if (IsDropDownOpen)
                        {
                            MoveFocusToDropDown();
                        }
                        else if (!IsDropDownOpen && InterceptKeyboardSelection && SelectionMode == SelectionMode.Single)
                        {
                            if (!isRTL)
                            {
                                SelectPrev();
                            }
                            else
                            {
                                // If it's RTL then Left should go the other direction
                                SelectNext();
                            }
                        }
                        handled = true;
                    }
                    break;

                case Key.PageUp:
                    if (IsDropDownOpen)
                    {
                        // At the moment this feature is not implemented for this control.
                        handled = true;
                    }
                    break;

                case Key.PageDown:
                    if (IsDropDownOpen)
                    {
                        // At the moment this feature is not implemented for this control.
                        handled = true;
                    }
                    break;

                case Key.Oem5:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        // At the moment this feature is not implemented for this control.
                        handled = true;
                    }
                    break;

                default:
                    handled = false;
                    break;
            }
            if (handled)
            {
                e.Handled = true;
            }
        }

        // adopted from original ComoBox
        private void SelectPrev()
        {
            if (!Items.IsEmpty)
            {
                // Search backwards from SelectedIndex - 1 but don't start before the beginning.
                // If SelectedIndex is less than 0, there is nothing to select before this item.
                if (SelectedIndex > 0)
                {
                    SelectItemHelper(SelectedIndex - 1, -1, -1);
                }
            }
        }


        // adopted from original ComoBox
        private void SelectNext()
        {
            int count = Items.Count;
            if (count > 0)
            {
                // Search forwards from SelectedIndex + 1 but don't start past the end.
                // If SelectedIndex is before the last item then there is potentially
                // something afterwards that we could select.
                if (SelectedIndex < count - 1)
                {
                    SelectItemHelper(SelectedIndex + 1, +1, count);
                }
            }
        }

        // adopted from original ComoBox
        private void SelectFirst()
        {
            SelectItemHelper(0, +1, Items.Count);
        }

        // adopted from original ComoBox
        private void SelectLast()
        {
            SelectItemHelper(Items.Count - 1, -1, -1);
        }

        // adopted from original ComoBox
        // Walk in the specified direction until we get to a selectable
        // item or to the stopIndex.
        // NOTE: stopIndex is not inclusive (it should be one past the end of the range)
        private void SelectItemHelper(int startIndex, int increment, int stopIndex)
        {
            Debug.Assert((increment > 0 && startIndex <= stopIndex) || (increment < 0 && startIndex >= stopIndex), "Infinite loop detected");

            for (int i = startIndex; i != stopIndex; i += increment)
            {
                // If the item is selectable and the wrapper is selectable, select it.
                // Need to check both because the user could set any combination of
                // IsSelectable and IsEnabled on the item and wrapper.
                object item = Items[i];
                DependencyObject container = ItemContainerGenerator.ContainerFromIndex(i);
                if (IsSelectableHelper(item) && IsSelectableHelper(container))
                {
                    SelectedIndex = i;
                    UpdateEditableText(true); // We force the update of the text
                    this.isUserdefinedTextInputPending = false;
                    break;
                }
            }
        }

        // adopted from original ComoBox
        private bool IsSelectableHelper(object o)
        {
            DependencyObject d = o as DependencyObject;
            // If o is not a DependencyObject, it is just a plain
            // object and must be selectable and enabled.
            if (d == null)
            {
                return true;
            }
            // It's selectable if IsSelectable is true and IsEnabled is true.
            return (bool)d.GetValue(IsEnabledProperty);
        }

        #endregion

        #region Events

        private void SelectedItemsImpl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            SyncSelectedItems(sender as IList, PART_PopupListBox?.SelectedItems, e);
        }


        private void SyncSelectedItems(IList sourceCollection, IList targetCollection, NotifyCollectionChangedEventArgs e)
        {
            if (IsSyncingSelectedItems || sourceCollection is null || targetCollection is null)
            {
                return;
            }

            IsSyncingSelectedItems = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        targetCollection.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        targetCollection.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.NewItems)
                    {
                        targetCollection.Add(item);
                    }
                    foreach (var item in e.OldItems)
                    {
                        targetCollection.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();

                    for (int i = 0; i < sourceCollection.Count; i++)
                    {
                        targetCollection.Add(sourceCollection[i]);
                    }
                    break;
            }

            this.UpdateDisplaySelectedItems();
            this.UpdateEditableText();
            this.UpdateHasCustomText(null);

            IsSyncingSelectedItems = false;
        }

        private void MultiSelectionComboBox_Loaded(object sender, EventArgs e)
        {
            this.Loaded -= this.MultiSelectionComboBox_Loaded;

            // If we have the ItemsSource set, we need to exit here. 
            if (PART_PopupListBox is null || ((PART_PopupListBox?.Items as IList)?.IsReadOnly ?? false) || BindingOperations.IsDataBound(PART_PopupListBox, ItemsSourceProperty))
            {
                return;
            }

            this.PART_PopupListBox.Items.Clear();
            foreach (var item in this.Items)
            {
                this.PART_PopupListBox.Items.Add(item);
            }
        }

        private void PART_EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.SelectItemsFromText(0);
        }

        private void PART_SelectedItemsPresenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If we have a ScrollViewer (ListBox has) we need to handle this event here as it will not be forwarded to the ToggleButton
            this.SetCurrentValue(IsDropDownOpenProperty, BooleanBoxes.Box(!this.IsDropDownOpen));
        }

        private void PART_SelectedItemsPresenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // We don't want the SelctedItems to be selectable. So anytime the selection will be changed we will reset it. 
            this.PART_SelectedItemsPresenter.SetCurrentValue(SelectedItemProperty, null);
        }

        private static void UpdateText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectionComboBox multiSelectionComboBox)
            {
                multiSelectionComboBox.UpdateEditableText();
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectionComboBox multiSelectionComboBox)
            {
                multiSelectionComboBox.UpdateHasCustomText(null);
                multiSelectionComboBox.isUserdefinedTextInputPending = true;

                // Select the items during typing if enabled
                if (multiSelectionComboBox.SelectItemsFromTextInputDelay >= 0)
                {
                    multiSelectionComboBox.SelectItemsFromText(multiSelectionComboBox.SelectItemsFromTextInputDelay);
                }
            }
        }

        private static void OnOrderSelectedItemsByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectionComboBox multiSelectionComboBox && !multiSelectionComboBox.HasCustomText)
            {
                multiSelectionComboBox.UpdateDisplaySelectedItems();
                multiSelectionComboBox.UpdateEditableText();
            }
        }

        /// <summary>Identifies the <see cref="AddingItem"/> routed event.</summary>
        public static readonly RoutedEvent AddingItemEvent = EventManager.RegisterRoutedEvent(
            nameof(AddingItem), RoutingStrategy.Bubble, typeof(AddingItemEventArgsHandler), typeof(MultiSelectionComboBox));

        /// <summary>
        ///     Occurs before a new object is added to the Items-List
        /// </summary>
        public event AddingItemEventArgsHandler AddingItem
        {
            add { AddHandler(AddingItemEvent, value); }
            remove { RemoveHandler(AddingItemEvent, value); }
        }

        /// <summary>Identifies the <see cref="AddedItem"/> routed event.</summary>
        public static readonly RoutedEvent AddedItemEvent = EventManager.RegisterRoutedEvent(
            nameof(AddedItem), RoutingStrategy.Bubble, typeof(AddedItemEventArgsHandler), typeof(MultiSelectionComboBox));

        /// <summary>
        ///     Occurs before a new object is added to the Items-List
        /// </summary>
        public event AddedItemEventArgsHandler AddedItem
        {
            add { AddHandler(AddedItemEvent, value); }
            remove { RemoveHandler(AddedItemEvent, value); }
        }

        #endregion
    }
}