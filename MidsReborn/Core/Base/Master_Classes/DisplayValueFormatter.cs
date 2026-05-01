using System;
using System.Globalization;

namespace Mids_Reborn.Core.Base.Master_Classes
{
    internal static class DisplayValueFormatter
    {
        public static string FormatNumber(float value)
        {
            return FormatNumber((double)value);
        }

        public static string FormatNumber(double value)
        {
            return FormatNumber(value, Math.Abs(value) < 100d ? 2 : 1);
        }

        public static string FormatNumber(float value, int maxDecimal)
        {
            return FormatNumber((double)value, maxDecimal);
        }

        public static string FormatNumber(double value, int maxDecimal)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return value.ToString(CultureInfo.CurrentCulture);
            }

            var integralFormat = Math.Abs(value) >= 10d ? "###0" : "0";
            if (maxDecimal <= 0)
            {
                return value.ToString(integralFormat, CultureInfo.CurrentCulture);
            }

            return value.ToString($"{integralFormat}.{new string('#', maxDecimal)}", CultureInfo.CurrentCulture);
        }

        public static string FormatPercentFromScale(float scale)
        {
            return FormatPercentValue(scale * 100f);
        }

        public static string FormatPercentFromScale(double scale)
        {
            return FormatPercentValue(scale * 100d);
        }

        public static string FormatPercentFromScale(float scale, int maxDecimal)
        {
            return FormatPercentValue(scale * 100f, maxDecimal);
        }

        public static string FormatPercentFromScale(double scale, int maxDecimal)
        {
            return FormatPercentValue(scale * 100d, maxDecimal);
        }

        public static string FormatPercentValue(float percent)
        {
            return FormatNumber(percent);
        }

        public static string FormatPercentValue(double percent)
        {
            return FormatNumber(percent);
        }

        public static string FormatPercentValue(float percent, int maxDecimal)
        {
            return FormatNumber(percent, maxDecimal);
        }

        public static string FormatPercentValue(double percent, int maxDecimal)
        {
            return FormatNumber(percent, maxDecimal);
        }

        public static string FormatSeconds(float seconds)
        {
            return FormatNumber(seconds);
        }

        public static string FormatSeconds(double seconds)
        {
            return FormatNumber(seconds);
        }

        public static string FormatSeconds(float seconds, int maxDecimal)
        {
            return FormatNumber(seconds, maxDecimal);
        }

        public static string FormatSeconds(double seconds, int maxDecimal)
        {
            return FormatNumber(seconds, maxDecimal);
        }

        public static string FormatRate(float value)
        {
            return FormatNumber(value);
        }

        public static string FormatRate(double value)
        {
            return FormatNumber(value);
        }

        public static string FormatRate(float value, int maxDecimal)
        {
            return FormatNumber(value, maxDecimal);
        }

        public static string FormatRate(double value, int maxDecimal)
        {
            return FormatNumber(value, maxDecimal);
        }

        public static string FormatMagnitude(float value)
        {
            return FormatNumber(value);
        }

        public static string FormatMagnitude(double value)
        {
            return FormatNumber(value);
        }

        public static string FormatMagnitude(float value, int maxDecimal)
        {
            return FormatNumber(value, maxDecimal);
        }

        public static string FormatMagnitude(double value, int maxDecimal)
        {
            return FormatNumber(value, maxDecimal);
        }

        public static string FormatDistance(float value)
        {
            return FormatNumber(value);
        }

        public static string FormatDistance(double value)
        {
            return FormatNumber(value);
        }

        public static string FormatDistance(float value, int maxDecimal)
        {
            return FormatNumber(value, maxDecimal);
        }

        public static string FormatDistance(double value, int maxDecimal)
        {
            return FormatNumber(value, maxDecimal);
        }
    }
}
