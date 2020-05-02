namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string @string) => string.IsNullOrEmpty(@string);

        public static bool IsNotEmpty(this string @string) => !string.IsNullOrEmpty(@string);
    }
}