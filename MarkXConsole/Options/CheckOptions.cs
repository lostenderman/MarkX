using CommandLine;

namespace MarkXConsole
{
    [Verb("check", HelpText = "Check file(s) and directories of files.")]
    public class CheckOptions : Options
    {
        [Option('R', "result", Required = false, HelpText = "Test file containing an expected result of parsing.")]
        public string? Result { get; set; }

        [Option('r', "own-result", Required = false, HelpText = "Prefer own expected result over a shared one.")]
        public bool OwnResult { get; set; }
    }
}