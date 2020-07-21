using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace TimsWpfControls
{
    public class DataGridIntellisenseTextboxColumn : DataGridTextColumn
    {
        private static Style _defaultEditingElementStyle;

        static DataGridIntellisenseTextboxColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridIntellisenseTextboxColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridIntellisenseTextboxColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        #region Styles


        /// <summary>
        ///     The default value of the EditingElementStyle property.
        ///     This value can be used as the BasedOn for new styles.
        /// </summary>
        public static new Style DefaultEditingElementStyle
        {
            get
            {
                if (_defaultEditingElementStyle == null)
                {
                    Style style = new Style(typeof(IntellisenseTextBox), (Style)Application.Current.Resources["Tims.Styles.IntellisenseTextBox"]);

                    style.Setters.Add(new Setter(IntellisenseTextBox.BorderThicknessProperty, new Thickness(0.0)));
                    style.Setters.Add(new Setter(IntellisenseTextBox.PaddingProperty, new Thickness(0.0)));
                    style.Setters.Add(new Setter(IntellisenseTextBox.MinHeightProperty, 1d));
                    style.Setters.Add(new Setter(IntellisenseTextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center));

                    style.Seal();
                    _defaultEditingElementStyle = style;
                }

                return _defaultEditingElementStyle;
            }
        }

        #endregion
        #region Generation

        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            TextBlock textBlock = new TextBlock();

            SyncProperties(textBlock);

            ApplyStyle(/* isEditing = */ false, /* defaultToElementStyle = */ false, textBlock);
            ApplyBinding(textBlock, TextBlock.TextProperty);

            return textBlock;
        }

        /// <summary>
        ///     Called when a cell has just switched to edit mode.
        /// </summary>
        /// <param name="editingElement">A reference to element returned by GenerateEditingElement.</param>
        /// <param name="editingEventArgs">The event args of the input event that caused the cell to go into edit mode. May be null.</param>
        /// <returns>The unedited value of the cell.</returns>
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            if (editingElement is IntellisenseTextBox textBox)
            {
                textBox.Focus();
                Keyboard.Focus(textBox);

                // For now this does not work

                //textBox.Dispatcher.BeginInvoke(
                //    DispatcherPriority.ContextIdle,
                //    (DispatcherOperationCallback)delegate (object arg)
                //    {
                //        var intellisenseTB = (IntellisenseTextBox)arg;
                //        intellisenseTB.SetCurrentValue(IntellisenseTextBox.IsIntellisensePopupOpenProperty, true);
                //        return null;
                //    },
                //    textBox);

                string originalValue = textBox.Text;

                if (editingEventArgs is TextCompositionEventArgs textArgs)
                {
                    // If text input started the edit, then replace the text with what was typed.
                    string inputText = ConvertTextForEdit(textArgs.Text);
                    textBox.Text = inputText;

                    // Place the caret after the end of the text.
                    textBox.Select(inputText.Length, 0);
                }
                else
                {
                    // If a mouse click started the edit, then place the caret under the mouse.
                    if ((editingEventArgs is MouseButtonEventArgs) || !PlaceCaretOnTextBox(textBox, Mouse.GetPosition(textBox)))
                    {
                        // If the mouse isn't over the textbox or something else started the edit, then select the text.
                        textBox.SelectAll();
                    }
                }

                return originalValue;
            }

            return null;
        }

        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            IntellisenseTextBox textBox = new IntellisenseTextBox();

            SyncProperties(textBox);

            ApplyStyle(/* isEditing = */ true, /* defaultToElementStyle = */ false, textBox);
            ApplyBinding(textBox, TextBox.TextProperty);

            return textBox;
        }

        // convert text the user has typed into the appropriate string to enter into the editable TextBox
        string ConvertTextForEdit(string s)
        {
            // Backspace becomes the empty string
            if (s == "\b")
            {
                s = String.Empty;
            }

            return s;
        }

        private static bool PlaceCaretOnTextBox(IntellisenseTextBox textBox, Point position)
        {
            int characterIndex = textBox.GetCharacterIndexFromPoint(position, /* snapToText = */ false);
            if (characterIndex >= 0)
            {
                textBox.Select(characterIndex, 0);
                return true;
            }

            return false;
        }

        private void SyncProperties(FrameworkElement e)
        {
            SyncColumnProperty(this, e, TextElement.FontFamilyProperty, FontFamilyProperty);
            SyncColumnProperty(this, e, TextElement.FontSizeProperty, FontSizeProperty);
            SyncColumnProperty(this, e, TextElement.FontStyleProperty, FontStyleProperty);
            SyncColumnProperty(this, e, TextElement.FontWeightProperty, FontWeightProperty);
            SyncColumnProperty(this, e, TextElement.ForegroundProperty, ForegroundProperty);
            SyncColumnProperty(this, e, IntellisenseTextBox.ContentAssistSourceProperty, ContentAssistSourceProperty);
            SyncColumnProperty(this, e, IntellisenseTextBox.MatchBeginningProperty, MatchBeginningProperty);
            SyncColumnProperty(this, e, IntellisenseTextBox.SuffixAfterInsertProperty, SuffixAfterInsertProperty);
            SyncColumnProperty(this, e, IntellisenseTextBox.DelimiterProperty, DelimiterProperty);
        }

        internal static void SyncColumnProperty(DependencyObject column, DependencyObject content, DependencyProperty contentProperty, DependencyProperty columnProperty)
        {
            if (IsDefaultValue(column, columnProperty))
            {
                content.ClearValue(contentProperty);
            }
            else
            {
                content.SetValue(contentProperty, column.GetValue(columnProperty));
            }
        }
        internal static bool IsDefaultValue(DependencyObject d, DependencyProperty dp)
        {
            return DependencyPropertyHelper.GetValueSource(d, dp).BaseValueSource == BaseValueSource.Default;
        }

        internal void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
        {
            Style style = PickStyle(isEditing, defaultToElementStyle);
            if (style != null)
            {
                element.Style = style;
            }
        }

        private Style PickStyle(bool isEditing, bool defaultToElementStyle)
        {
            Style style = isEditing ? EditingElementStyle : ElementStyle;
            if (isEditing && defaultToElementStyle && (style == null))
            {
                style = ElementStyle;
            }

            if (isEditing && style.TargetType == typeof(TextBox))
            {
                style = DefaultEditingElementStyle;
            }

            return style;
        }

        /// <summary>
        ///     Assigns the Binding to the desired property on the target object.
        /// </summary>
        internal void ApplyBinding(DependencyObject target, DependencyProperty property)
        {
            BindingBase binding = Binding;
            if (binding != null)
            {
                BindingOperations.SetBinding(target, property, binding);
            }
            else
            {
                BindingOperations.ClearBinding(target, property);
            }
        }

        #endregion

        #region IntellisenseProperties


        public IEnumerable<object> ContentAssistSource
        {
            get { return (IEnumerable<object>)GetValue(ContentAssistSourceProperty); }
            set { SetValue(ContentAssistSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentAssistSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentAssistSourceProperty =
            DependencyProperty.Register("ContentAssistSource", typeof(IEnumerable<object>), typeof(DataGridIntellisenseTextboxColumn), new PropertyMetadata(null));


        public bool MatchBeginning
        {
            get { return (bool)GetValue(MatchBeginningProperty); }
            set { SetValue(MatchBeginningProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MatchBeginning.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MatchBeginningProperty =
            DependencyProperty.Register("MatchBeginning", typeof(bool), typeof(DataGridIntellisenseTextboxColumn), new PropertyMetadata(true));




        public string SuffixAfterInsert
        {
            get { return (string)GetValue(SuffixAfterInsertProperty); }
            set { SetValue(SuffixAfterInsertProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SuffixAfterInsert.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuffixAfterInsertProperty =
            DependencyProperty.Register("SuffixAfterInsert", typeof(string), typeof(DataGridIntellisenseTextboxColumn), new PropertyMetadata(null));




        public object Delimiter
        {
            get { return (object)GetValue(DelimiterProperty); }
            set { SetValue(DelimiterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Delimiter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelimiterProperty =
            DependencyProperty.Register("Delimiter", typeof(object), typeof(DataGridIntellisenseTextboxColumn), new PropertyMetadata(".,; \n\r"));



        #endregion
    }
}
