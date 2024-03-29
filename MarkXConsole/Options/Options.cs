using CommandLine;

namespace MarkXConsole
{
    public class Options
    {
        [Option('I', "input", Required = true, HelpText = "Input file(s) and directories of files to be processed. Directories will be recursively searched for files.")]
        public IEnumerable<string>? Input { get; set; }

        [Option('u', "ungroup-sections", Required = false, HelpText = "Process section groups as a single array of tests.")]
        public bool UngroupSections { get; set; }

        [Option('c', "code-indented", Required = false, HelpText = "Render the default code element as indented code, rather than fenced code")]
        public bool IndentCode { get; set; }

        [Option('e', "extensions", Required = false, HelpText = "Enable custom extensions.")]
        public IEnumerable<string>? Extensions { get; set; }

        [Option('s', "isolate-sections", Required = false, HelpText = "Supress merging of identically named sections in different files.")]
        public bool IsolateSections { get; set; }

        [Option('q', "quiet", Required = false, HelpText = "Suppress printing results to console output.")]
        public bool Quiet { get; set; }
    }
}