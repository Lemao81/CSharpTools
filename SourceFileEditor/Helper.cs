using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SourceFileEditor
{
    public static class Helper
    {
        public static readonly DirectoryInfo SourceFolder = new DirectoryInfo(@"C:\Repos\RR-Backend");

        public static List<FileInfo> GetSourceFiles(string pattern) => SourceFolder.GetFiles($"{pattern}.cs", SearchOption.AllDirectories).ToList();

        public static IEnumerable<string> GetModuleNames(params ModuleType[] exclude) =>
            exclude.Length == 0 ? Enum.GetNames(typeof(ModuleType)) : Enum.GetNames(typeof(ModuleType)).Where(n => exclude.All(m => m.ToString() != n));

        public static void Print(string str = null)
        {
            if (string.IsNullOrEmpty(str))
                Console.WriteLine();
            else
                Console.WriteLine(str);
        }
    }
}
