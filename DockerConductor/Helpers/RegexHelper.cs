namespace DockerConductor.Helpers
{
    public static class RegexHelper
    {
        public static string InBetweenRegexPattern(string from, string to) => $"(?<={from}).*(?={to})";
    }
}
