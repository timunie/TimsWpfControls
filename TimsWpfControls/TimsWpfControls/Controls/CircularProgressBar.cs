using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TimsWpfControls.ExtensionMethods;

namespace TimsWpfControls
{
    [TemplatePart(Name = "PART_ArcSegment", Type = typeof(PathFigure))]
    public class CircularProgressBar : ContentControl
    {
        private PathFigure PART_ArcSegment;

        // Using a DependencyProperty as the backing store for StartDegrees.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartDegreesProperty = DependencyProperty.Register("StartDegrees", typeof(double), typeof(CircularProgressBar), new FrameworkPropertyMetadata(-90d, FrameworkPropertyMetadataOptions.AffectsRender));

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularProgressBar), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));

        // Using a DependencyProperty as the backing store for IsFilled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFilledProperty = DependencyProperty.Register("IsFilled", typeof(bool), typeof(CircularProgressBar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        // Using a DependencyProperty as the backing store for IsIndeterminate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(CircularProgressBar), new PropertyMetadata(false));

        #region Implementation of Properties

        #region Events

        /// <summary>
        /// Event correspond to Value changed event
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(CircularProgressBar));

        /// <summary>
        /// Add / Remove ValueChangedEvent handler
        /// </summary>
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<double> ValueChanged { add { AddHandler(ValueChangedEvent, value); } remove { RemoveHandler(ValueChangedEvent, value); } }

        #endregion Events

        #region Properties

        /// <summary>
        ///     The DependencyProperty for the Minimum property.
        ///     Flags:              none
        ///     Default Value:      0
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
                DependencyProperty.Register(
                        "Minimum",
                        typeof(double),
                        typeof(CircularProgressBar),
                        new FrameworkPropertyMetadata(
                                0.0d,
                                new PropertyChangedCallback(OnMinimumChanged)),
                        new ValidateValueCallback(IsValidDoubleValue));

        /// <summary>
        ///     Minimum restricts the minimum value of the Value property
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        ///     Called when MinimumProperty is changed on "d."
        /// </summary>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularProgressBar ctrl = (CircularProgressBar)d;

            //RangeBaseAutomationPeer peer = UIElementAutomationPeer.FromElement(ctrl) as RangeBaseAutomationPeer;
            //if (peer != null)
            //{
            //    peer.RaiseMinimumPropertyChangedEvent((double)e.OldValue, (double)e.NewValue);
            //}

            ctrl.CoerceValue(MaximumProperty);
            ctrl.CoerceValue(ValueProperty);
            ctrl.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        ///     This method is invoked when the Minimum property changes.
        /// </summary>
        /// <param name="oldMinimum">The old value of the Minimum property.</param>
        /// <param name="newMinimum">The new value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        /// <summary>
        ///     The DependencyProperty for the Maximum property.
        ///     Flags:              none
        ///     Default Value:      1
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
                DependencyProperty.Register(
                        "Maximum",
                        typeof(double),
                        typeof(CircularProgressBar),
                        new FrameworkPropertyMetadata(
                                1.0d,
                                new PropertyChangedCallback(OnMaximumChanged),
                                new CoerceValueCallback(CoerceMaximum)),
                        new ValidateValueCallback(IsValidDoubleValue));

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            CircularProgressBar ctrl = (CircularProgressBar)d;
            double min = ctrl.Minimum;
            if ((double)value < min)
            {
                return min;
            }
            return value;
        }

        /// <summary>
        ///     Maximum restricts the maximum value of the Value property
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        ///     Called when MaximumProperty is changed on "d."
        /// </summary>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularProgressBar ctrl = (CircularProgressBar)d;

            //RangeBaseAutomationPeer peer = UIElementAutomationPeer.FromElement(ctrl) as RangeBaseAutomationPeer;
            //if (peer != null)
            //{
            //    peer.RaiseMaximumPropertyChangedEvent((double)e.OldValue, (double)e.NewValue);
            //}

            ctrl.CoerceValue(ValueProperty);
            ctrl.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        ///     This method is invoked when the Maximum property changes.
        /// </summary>
        /// <param name="oldMaximum">The old value of the Maximum property.</param>
        /// <param name="newMaximum">The new value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        /// <summary>
        ///     The DependencyProperty for the Value property.
        ///     Flags:              None
        ///     Default Value:      0
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(
                        "Value",
                        typeof(double),
                        typeof(CircularProgressBar),
                        new FrameworkPropertyMetadata(
                                0.0d,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                new PropertyChangedCallback(OnValueChanged),
                                new CoerceValueCallback(ConstrainToRange)),
                        new ValidateValueCallback(IsValidDoubleValue));

        // made this internal because Slider wants to leverage it
        internal static object ConstrainToRange(DependencyObject d, object value)
        {
            CircularProgressBar ctrl = (CircularProgressBar)d;
            double min = ctrl.Minimum;
            double v = (double)value;
            if (v < min)
            {
                return min;
            }

            double max = ctrl.Maximum;
            if (v > max)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        ///     Value property
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        ///     Called when ValueID is changed on "d."
        /// </summary>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularProgressBar ctrl = (CircularProgressBar)d;

            //RangeBaseAutomationPeer peer = UIElementAutomationPeer.FromElement(ctrl) as RangeBaseAutomationPeer;
            //if (peer != null)
            //{
            //    peer.RaiseValuePropertyChangedEvent((double)e.OldValue, (double)e.NewValue);
            //}

            ctrl.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        ///     This method is invoked when the Value property changes.
        /// </summary>
        /// <param name="oldValue">The old value of the Value property.</param>
        /// <param name="newValue">The new value of the Value property.</param>
        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventArgs<double> args = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
            args.RoutedEvent = CircularProgressBar.ValueChangedEvent;
            RaiseEvent(args);
        }

        /// <summary>
        /// Validate input value in RangeBase (Minimum, Maximum, and Value).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns False if value is NaN or NegativeInfinity or PositiveInfinity. Otherwise, returns True.</returns>
        private static bool IsValidDoubleValue(object value)
        {
            double d = (double)value;

            return !(double.IsNaN(d) || double.IsInfinity(d));
        }

        /// <summary>
        /// Validate input value in RangeBase (SmallChange and LargeChange).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns False if value is NaN or NegativeInfinity or PositiveInfinity or negative. Otherwise, returns True.</returns>
        private static bool IsValidChange(object value)
        {
            double d = (double)value;

            return IsValidDoubleValue(value) && d >= 0.0;
        }

        /// <summary>
        /// Gets or Sets if the Progressring is Indeterminate
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        #endregion Properties

        #endregion Implementation of Properties

        public double StartDegrees
        {
            get { return (double)GetValue(StartDegreesProperty); }
            set { SetValue(StartDegreesProperty, value); }
        }

        public double SweepDegrees => (Value - Minimum) / (Maximum - Minimum) * 360;

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public bool IsFilled
        {
            get { return (bool)GetValue(IsFilledProperty); }
            set { SetValue(IsFilledProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_ArcSegment = (PathFigure)GetTemplateChild(nameof(PART_ArcSegment));

            ValueChanged += (s, e) => UpdateProgress();
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            // degrees to radians conversion
            double startRadians = StartDegrees * Math.PI / 180.0;
            double sweepRadians = SweepDegrees * Math.PI / 180.0;

            // x and y radius
            double dx = (ActualWidth - StrokeThickness) / 2;
            double dy = (ActualHeight - StrokeThickness) / 2;

            dx = dx < 0 ? 0 : dx;
            dy = dy < 0 ? 0 : dy;

            Size EllipseSize = new Size(dx, dy);

            // determine the start point
            Point StartPoint = new Point(ActualWidth / 2 + Math.Cos(startRadians) * dx,
                                         ActualHeight / 2 + Math.Sin(startRadians) * dy);

            // determine the end point
            Point EndPoint = new Point(ActualWidth / 2 + Math.Cos(startRadians + sweepRadians) * dx,
                                       ActualHeight / 2 + Math.Sin(startRadians + sweepRadians) * dy);

            // draw the arc
            bool isLargeArc = Math.Abs(SweepDegrees) > 180;
            SweepDirection sweepDirection = SweepDegrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

            PART_ArcSegment.Segments.Clear();
            PART_ArcSegment.StartPoint = StartPoint;
            if (SweepDegrees.ApproximateEqualTo(360))
            {
                EndPoint = new Point(ActualWidth / 2 + Math.Cos(startRadians + Math.PI) * dx,
                                     ActualHeight / 2 + Math.Sin(startRadians + Math.PI) * dy);
                PART_ArcSegment.Segments.Add(new ArcSegment(EndPoint, EllipseSize, 0, false, sweepDirection, true));
                PART_ArcSegment.Segments.Add(new ArcSegment(StartPoint, EllipseSize, 0, true, sweepDirection, true));
            }
            else if (SweepDegrees.ApproximateEqualTo(0))
            {
                // Do not draw anyting if there is nothing to see
            }
            else
            {
                PART_ArcSegment.Segments.Add(new ArcSegment(EndPoint, EllipseSize, 0, isLargeArc, sweepDirection, true));
                if (IsFilled)
                {
                    PART_ArcSegment.Segments.Add(new LineSegment(new Point(dx + StrokeThickness / 2, dy + StrokeThickness / 2), true));
                    PART_ArcSegment.Segments.Add(new LineSegment(StartPoint, true));
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            UpdateProgress();
            base.OnRender(drawingContext);
        }
    }
}