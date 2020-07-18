using MahApps.Metro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TimsWpfControls.ExtensionMethods;

namespace TimsWpfControls
{
    public class IntellisenseTextBox : TextBox
    {
        static IntellisenseTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntellisenseTextBox), new FrameworkPropertyMetadata(typeof(IntellisenseTextBox)));
        }

        // Templateparts
        private Popup PART_IntellisensePopup;

        private ListBox PART_IntellisenseListBox;

        // Using a DependencyProperty as the backing store for ContentAssistSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentAssistSourceProperty =
            DependencyProperty.Register("ContentAssistSource", typeof(IEnumerable<object>), typeof(IntellisenseTextBox), new UIPropertyMetadata(null, OnContentAssistSourceChanged));

        // Using a DependencyProperty as the backing store for MatchBeginning.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MatchBeginningProperty =
            DependencyProperty.Register("MatchBeginning", typeof(bool), typeof(IntellisenseTextBox), new PropertyMetadata(true));

        public IEnumerable<object> ContentAssistSource
        {
            get { return (IEnumerable<object>)GetValue(ContentAssistSourceProperty); }
            set { SetValue(ContentAssistSourceProperty, value); }
        }

        public bool MatchBeginning
        {
            get { return (bool)GetValue(MatchBeginningProperty); }
            set { SetValue(MatchBeginningProperty, value); }
        }

        public IEnumerable<string> ConentAssistSource_ResultView
        {
            get { return (IEnumerable<string>)GetValue(ConentAssistSource_ResultViewProperty); }
            set { SetValue(ConentAssistSource_ResultViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConentAssistSource_ResultView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConentAssistSource_ResultViewProperty =
            DependencyProperty.Register("ConentAssistSource_ResultView", typeof(IEnumerable<string>), typeof(IntellisenseTextBox), new PropertyMetadata(default(IEnumerable<string>)));

        //public ICollectionView ContentAssistSource_CollectionView { get; private set; }

        private static void OnContentAssistSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IntellisenseTextBox intellisenseTextBox)
            {
                intellisenseTextBox.Update_AssistSourceResultView();
            }
        }

        // Using a DependencyProperty as the backing store for SuffixAfterInsert.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuffixAfterInsertProperty =
            DependencyProperty.Register("SuffixAfterInsert", typeof(string), typeof(IntellisenseTextBox), new PropertyMetadata(null));

        public string SuffixAfterInsert
        {
            get { return (string)GetValue(SuffixAfterInsertProperty); }
            set { SetValue(SuffixAfterInsertProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_IntellisensePopup = (Popup)GetTemplateChild(nameof(PART_IntellisensePopup));
            this.PART_IntellisensePopup.Opened += PART_IntellisensePopup_Opened;
            this.PART_IntellisensePopup.Closed += PART_IntellisensePopup_Closed;

            this.PART_IntellisenseListBox = (ListBox)GetTemplateChild(nameof(PART_IntellisenseListBox));
            this.PART_IntellisenseListBox.MouseDoubleClick += PART_IntellisenseListBox_MouseDoubleClick;
            this.PART_IntellisenseListBox.PreviewKeyDown += PART_IntellisenseListBox_PreviewKeyDown;
        }

        private void PART_IntellisensePopup_Closed(object sender, EventArgs e)
        {
            sbLastWords.Clear();
            IsAssistKeyPressed = false;
            Update_AssistSourceResultView();
        }

        private void PART_IntellisensePopup_Opened(object sender, EventArgs e)
        {
            sbLastWords.Clear();
            UpdatePopupPosition();
        }

        private void UpdatePopupPosition()
        {
            var pos = GetRectFromCharacterIndex(this.CaretIndex);
            PART_IntellisensePopup.Placement = PlacementMode.RelativePoint;
            PART_IntellisensePopup.PlacementTarget = this;
            PART_IntellisensePopup.HorizontalOffset = pos.Left;
            PART_IntellisensePopup.VerticalOffset = pos.Top + pos.Height;
        }

        private void PART_IntellisenseListBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // just run this code if we have the dropdown open
            if (!PART_IntellisensePopup.IsOpen) return;

            //if Enter\Tab\Space key is pressed, insert current selected item to richtextbox
            if (e.Key == Key.Enter || e.Key == Key.Tab || e.Key == Key.Space)
            {
                InsertAssistWord();
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                //Baskspace key is pressed, set focus to richtext box
                if (sbLastWords.Length >= 1)
                {
                    sbLastWords.Remove(sbLastWords.Length - 1, 1);
                }
                this.Focus();
            }
        }

        private void PART_IntellisenseListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InsertAssistWord();
        }

        #region Content Assist

        private bool IsAssistKeyPressed = false;
        private readonly StringBuilder sbLastWords = new StringBuilder();

        private bool InsertAssistWord()
        {
            bool isInserted = false;
            if (PART_IntellisenseListBox.SelectedIndex != -1)
            {
                string selectedString = (string)PART_IntellisenseListBox.SelectedItem;
                selectedString += SuffixAfterInsert;

                this.InsertText(selectedString);

                isInserted = true;

                PART_IntellisensePopup.IsOpen = false;
                sbLastWords.Clear();
                IsAssistKeyPressed = false;
            }
            return isInserted;
        }

        public void InsertText(string text)
        {
            Focus();
            var _newCaretIndex = CaretIndex - sbLastWords.Length;
            SetValue(TextProperty, Text.Remove(_newCaretIndex, sbLastWords.Length));
            SetValue(TextProperty, Text.Insert(_newCaretIndex, text));

            CaretIndex = _newCaretIndex + text.Length;

            sbLastWords.Clear();
            PART_IntellisensePopup.IsOpen = false;
            Update_AssistSourceResultView();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!PART_IntellisensePopup.IsOpen)
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Space)
                {
                    if (CaretIndex > 0 && sbLastWords.Length == 0 && Text.Length > 0 && !char.IsWhiteSpace(Text, CaretIndex - 1))
                    {
                        sbLastWords.Append(Text.GetStringToTheRight(CaretIndex, new char[] { ' ', '\r', '\n' }));
                        Update_AssistSourceResultView();
                    }
                    PART_IntellisensePopup.IsOpen = true;
                    IsAssistKeyPressed = true;
                    e.Handled = true;
                    return;
                }
                else
                {
                    base.OnPreviewKeyDown(e);
                }
                return;
            }

            Update_AssistSourceResultView();

            switch (e.Key)
            {
                case Key.Back:
                    if (sbLastWords.Length > 0)
                    {
                        sbLastWords.Remove(sbLastWords.Length - 1, 1);
                        Update_AssistSourceResultView();
                    }
                    else
                    {
                        IsAssistKeyPressed = false;
                        sbLastWords.Clear();
                        PART_IntellisensePopup.IsOpen = false;
                    }
                    break;

                case Key.Enter:
                case Key.Tab:
                    if (InsertAssistWord())
                    {
                        e.Handled = true;
                    }
                    break;

                case Key.Down:
                    if (PART_IntellisenseListBox.SelectedIndex < PART_IntellisenseListBox.Items.Count - 1)
                        PART_IntellisenseListBox.SelectedIndex += 1;
                    e.Handled = true;
                    break;

                case Key.Up:
                    if (PART_IntellisenseListBox.SelectedIndex > -1)
                        PART_IntellisenseListBox.SelectedIndex -= 1;
                    e.Handled = true;
                    break;

                case Key.Space:
                case Key.Escape:
                    sbLastWords.Clear();
                    PART_IntellisensePopup.IsOpen = false;
                    break;

                default:
                    break;
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            if (PART_IntellisensePopup.IsOpen == false && e.Text.Length == 1 && e.Text != "\u001B")
            {
                sbLastWords.Clear();
                PART_IntellisensePopup.IsOpen = true;
                IsAssistKeyPressed = true;
                Update_AssistSourceResultView();
            }

            if (IsAssistKeyPressed)
            {
                if (sbLastWords.Length == 0 && Text.Length > 0 && !char.IsWhiteSpace(Text, CaretIndex - 1))
                {
                    sbLastWords.Append(Text.GetStringToTheRight(CaretIndex, new char[] { ' ', '\r', '\n' }));
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(e.Text)) sbLastWords.Append(e.Text);
                }
                Update_AssistSourceResultView();
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (!(PART_IntellisensePopup.IsKeyboardFocusWithin && this.IsKeyboardFocusWithin))
            {
                PART_IntellisensePopup.IsOpen = false;
                Update_AssistSourceResultView();
            }

            base.OnLostFocus(e);
        }

        private void Update_AssistSourceResultView()
        {
            if (!IsInitialized) return;

            var compareTo = sbLastWords.ToString();
            SetValue(ConentAssistSource_ResultViewProperty,
                    ContentAssistSource?.Where(x => IsMatch(x?.ToString(), compareTo))
                    .Select(x => x.ToString())
                    .OrderBy(x => x));

            if (ConentAssistSource_ResultView == null || !ConentAssistSource_ResultView.Any() )
            {
                PART_IntellisensePopup.IsOpen = false;
            }
        }

        private bool IsMatch(string str, string compareTo)
        {
            if (sbLastWords.Length == 0) return true;

            if (MatchBeginning)
            {
                return str?.StartsWith(compareTo, StringComparison.OrdinalIgnoreCase) ?? false;
            }
            else
            {
                return str?.IndexOf(compareTo.ToString(), StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        #endregion Content Assist
    }
}