using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Assets.Helpers
{
    public static class NumberHelpers
    {
        public static double StringToDouble(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return -55555.55555;
            NumberFormatInfo provider = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
            return Convert.ToDouble(input, provider);
        }
        public static bool IsAllDigits(string s) => int.TryParse(s, out int i);
    }
}