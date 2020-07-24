using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimsWpfControls
{
    public static class ScrollViewerHelper
    {
        // The following Propety was taken from here: https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/
        public static readonly DependencyProperty BubbleUpScrollEventToParentScrollviewerProperty = DependencyProperty.RegisterAttached("BubbleUpScrollEventToParentScrollviewer", typeof(bool), typeof(ScrollViewerHelper), new FrameworkPropertyMetadata(false, ScrollViewerHelper.OnBubbleUpScrollEventToParentScrollviewerPropertyChanged));


        /// <summary>Helper for getting <see cref="BubbleUpScrollEventToParentScrollviewerProperty"/> on <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to get <see cref="BubbleUpScrollEventToParentScrollviewerProperty"/> on.</param>
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static bool GetBubbleUpScrollEventToParentScrollviewer(DependencyObject obj)
        {
            return (bool)obj.GetValue(BubbleUpScrollEventToParentScrollviewerProperty);
        }


        /// <summary>Helper for setting <see cref="BubbleUpScrollEventToParentScrollviewerProperty"/> on <paramref name="obj"/>.</summary>
        /// <param name="obj"><see cref="DependencyObject"/> to set <see cref="BubbleUpScrollEventToParentScrollviewerProperty"/> on.</param>
        /// <param name="value">BubbleUpScrollEventToParentScrollviewerProperty property value.</param>
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static void SetBubbleUpScrollEventToParentScrollviewer(DependencyObject obj, bool value)
        {
            obj.SetValue(BubbleUpScrollEventToParentScrollviewerProperty, value);
        }


        public static void OnBubbleUpScrollEventToParentScrollviewerPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ScrollViewer viewer))
                throw new ArgumentException("The dependency property can only be attached to a ScrollViewer", "sender");


            if ((bool)e.NewValue == true)
                viewer.PreviewMouseWheel += HandlePreviewMouseWheel;

            else if ((bool)e.NewValue == false)
                viewer.PreviewMouseWheel -= HandlePreviewMouseWheel;

        }

        private static List<MouseWheelEventArgs> _reentrantList = new List<MouseWheelEventArgs>();

        private static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollControl = sender as ScrollViewer;

            if (!e.Handled && sender != null && !_reentrantList.Contains(e))
            {
                var previewEventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent, Source = sender
                };

                var originalSource = e.OriginalSource as UIElement;
                _reentrantList.Add(previewEventArg);
                originalSource.RaiseEvent(previewEventArg);
                _reentrantList.Remove(previewEventArg);

                // at this point if no one else handled the event in our children, we do our job
                if (!previewEventArg.Handled && ((e.Delta > 0 && scrollControl.VerticalOffset == 0)
                    || (e.Delta <= 0 && scrollControl.VerticalOffset >= scrollControl.ExtentHeight - scrollControl.ViewportHeight)))
                {
                    e.Handled = true;
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = sender;
                    var parent = (UIElement)((FrameworkElement)sender).Parent;
                    parent.RaiseEvent(eventArg);

                }

            }

        }

    }
}