namespace DockerConductor.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNull(this string str) => str == null;

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);
    }
}
