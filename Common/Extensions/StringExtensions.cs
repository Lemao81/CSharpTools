using System;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNotEmpty(this string str) => !string.IsNullOrEmpty(str);

        public static bool EqualsIgnoringCase(this string str, string other) => string.Equals(str, other, StringComparison.InvariantCultureIgnoreCase);
    }
}
