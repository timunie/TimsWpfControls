using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TimsWpfControls.ExtensionMethods;

namespace TimsWpfControls
{
    public class ArcSegment : Control
    {
        private PathFigure PART_ArcSegment;

        public static readonly DependencyProperty StartDegreesProperty = DependencyProperty.Register("StartDegrees", typeof(double), typeof(ArcSegment), new FrameworkPropertyMetadata(-90d, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty SweepDegreesProperty = DependencyProperty.Register("SweepDegrees", typeof(double), typeof(ArcSegment), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ArcSegment), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty IsFilledProperty = DependencyProperty.Register("IsFilled", typeof(bool), typeof(ArcSegment), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public double StartDegrees
        {
            get { return (double)GetValue(StartDegreesProperty); }
            set { SetValue(StartDegreesProperty, value); }
        }


        public double SweepDegrees
        {
            get { return (double)GetValue(SweepDegreesProperty); }
            set { SetValue(SweepDegreesProperty, value); }
        }


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

        private void UpdateArcSegment()
        {
            if (PART_ArcSegment is null) return;

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
                PART_ArcSegment.Segments.Add(new System.Windows.Media.ArcSegment(EndPoint, EllipseSize, 0, false, sweepDirection, true));
                PART_ArcSegment.Segments.Add(new System.Windows.Media.ArcSegment(StartPoint, EllipseSize, 0, true, sweepDirection, true));
            }
            else if (SweepDegrees.ApproximateEqualTo(0))
            {
                // Do not draw anyting if there is nothing to see
            }
            else
            {
                PART_ArcSegment.Segments.Add(new System.Windows.Media.ArcSegment(EndPoint, EllipseSize, 0, isLargeArc, sweepDirection, true));
                if (IsFilled)
                {
                    PART_ArcSegment.Segments.Add(new LineSegment(new Point(dx + StrokeThickness / 2, dy + StrokeThickness / 2), true));
                    PART_ArcSegment.Segments.Add(new LineSegment(StartPoint, true));
                }
            }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ArcSegment = (PathFigure)GetTemplateChild(nameof(PART_ArcSegment));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            UpdateArcSegment();
            base.OnRender(drawingContext);
        }
    }
}
