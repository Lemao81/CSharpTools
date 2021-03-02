using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SourceFileEditor
{
    public static class Extensions
    {
        public static List<FileInfo> WithoutPart(this IEnumerable<FileInfo> files, string part) =>
            files.Where(f => !f.FullName.Contains(part)).ToList();

        public static List<FileInfo> WithPart(this IEnumerable<FileInfo> files, string part) =>
            files.Where(f => f.FullName.Contains(part)).ToList();

        public static List<FileInfo> Matches(this IEnumerable<FileInfo> files, string pattern) =>
            files.Where(f => Regex.IsMatch(f.FullName, pattern)).ToList();

        public static Match Match(this FileInfo file, string pattern) => Regex.Match(file.FullName, pattern);

        public static int IndexOf(this List<string> lines, string part) => lines.FindIndex(l => l.Contains(part));

        public static bool HasUsing(this List<string> lines, string namespaceString) => lines.Any(l => l.Contains(namespaceString));

        public static void InsertUsing(this List<string> lines, string namespaceString) => lines.Insert(0, $"using {namespaceString};");

        public static void WriteAll(this List<string> lines, FileInfo file) => File.WriteAllLines(file.FullName, lines);

        public static List<string> ReadLines(this FileInfo file) => File.ReadLines(file.FullName).ToList();

        public static List<FileInfo> OfModule(this IEnumerable<FileInfo> files, string moduleName) => files.WithPart($".{moduleName}.").ToList();

        public static string LineWith(this List<string> lines, string part) => lines.SingleOrDefault(l => l.Contains(part)) ?? string.Empty;

        public static string Extract(this string str, string pattern)
        {
            var match = Regex.Match(str, pattern);
            if (!match.Success || match.Groups.Count != 2) throw new InvalidOperationException(match.ToString());

            return match.Groups[1].Value;
        }

        public static List<FileInfo> SortByName(this List<FileInfo> files) => files.OrderBy(f => f.Name).ToList();
    }
}
