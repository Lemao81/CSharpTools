using CommandLine;

namespace ResxTools
{
    public class Options
    {
        [Option('d', "duplicateKeyPath", HelpText = "Path to the file who's keys shall be searched for duplicates", Required = false)]
        public string FindDuplicateKeysFilePath { get; set; }
    }
}