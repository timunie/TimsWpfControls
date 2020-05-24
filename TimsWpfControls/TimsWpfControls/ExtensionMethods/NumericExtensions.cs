using System;

namespace TimsWpfControls.ExtensionMethods
{
    public static class NumericExtensions
    {
        /// <summary>
        /// Checks if two double values are "equal enough"
        /// </summary>
        /// <param name="value">The value to compare</param>
        /// <param name="ValueToCompare">The value to compare to</param>
        /// <param name="MaxDeviation">The tolerance. Default: 1e-5</param>
        /// <returns></returns>
        public static bool ApproximateEqualTo(this double value, double ValueToCompare, double MaxDeviation = 1e-5)
        {
            if (double.IsNaN(value) || double.IsNaN(ValueToCompare))
            {
                return value == ValueToCompare;
            }
            else
            {
                return Math.Abs(value - ValueToCompare) <= MaxDeviation;
            }
        }
    }
}