using CommandLine;

namespace MarkXConsole
{
    [Verb("parse", HelpText = "Parse file(s) and directories of files.")]
    public class ParseOptions : Options
    {
        [Option('O', "output", Required = true, HelpText = "Output file.")]
        public string? Output { get; set; }

        [Option('t', "tree", Required = false, HelpText = "Parse result into a file system tree. Default output format is JSON.")]
        public bool ParseToTree { get; set; }

        [Option('f', "file", Required = false, HelpText = "Parse result into a file. Default output format is JSON.")]
        public bool ParseToFile { get; set; }

        [Option('m', "exclude-markdown", Required = false, HelpText = "Exclude markdown input from parsed result if present.")]
        public bool ExcludeMarkdown { get; set; }

        [Option('i', "full-index", Required = false, HelpText = "Include the section name in the test file name.")]
        public bool FullIndex { get; set; }
    }
}