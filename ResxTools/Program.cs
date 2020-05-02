using System;
using System.Linq;
using System.Xml.Linq;
using CommandLine;
using Common.Extensions;

namespace ResxTools
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                if (options.FindDuplicateKeysFilePath.IsNotEmpty())
                {
                    PrintDuplicateKeys(options.FindDuplicateKeysFilePath);
                }
            });
        }

        private static void PrintDuplicateKeys(string filePath)
        {
            XDocument document;
            try
            {
                document = XDocument.Load(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Parsing file failed!");
                Console.WriteLine();
                Console.WriteLine(e);
                return;
            }

            var dataElements = document.Element("root")?.Elements("data").ToList();
            if (dataElements == null || !dataElements.Any())
            {
                Console.WriteLine("No data elements available!");
                return;
            }

            var dataGroups = dataElements.GroupBy(_ => _.Attribute("name")?.Value, _ => _);
            var duplicates = dataGroups.Where(_ => _.Count() > 1).ToList();
            if (!duplicates.Any())
            {
                Console.WriteLine("No duplicate keys!");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Duplicate keys:");
                Console.WriteLine();
                foreach (var duplicate in duplicates)
                {
                    Console.WriteLine(duplicate.Key);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Done!");
        }
    }
}