using System;
using System.Globalization;
#if UNITY_EDITOR
#endif
namespace FriendsGamesTools
{
    public static partial class Utils
    {
        public static NumberFormatInfo useDot = new NumberFormatInfo { NumberDecimalSeparator = "." };
        public static string ToString(this float amount, int decimalPlaces)
        {
            return Math.Round(amount, decimalPlaces).ToString(useDot);
        }
        public static string ToString(this double amount, int decimalPlaces)
        {
            return Math.Round(amount, decimalPlaces).ToString(useDot);
        }
    }
}