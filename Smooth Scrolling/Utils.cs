using System;

namespace SmoothScrollingExtension
{
    /// <summary>
    /// Contains generic functionalities
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Linear Interpolation implementation
        /// </summary>
        /// <param name="a">from</param>
        /// <param name="b">to</param>
        /// <param name="amount">The amount (0 ~ 1)</param>
        public static double Lerp(double a, double b, double amount)
        {
            return a + (b - a) * amount;
        }

        public static double GetCurrentTime()
        {
            var time = (Environment.TickCount * 1E-03);
            return time;
        }
    }
}