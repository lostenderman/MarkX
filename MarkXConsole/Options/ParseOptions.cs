using CommandLine;

namespace MarkX.ConsoleUI
{
	[Verb("parse", HelpText = "Parse file.")]
	public class ParseOptions : Options
	{
		[Option('t', "tree", Required = false, HelpText = "Parse result into a file system tree.")]
		public bool ParseToTree { get; set; }

		[Option('f', "file", Required = false, HelpText = "Parse result into a file.")]
		public bool ParseToFile { get; set; }

		[Option('m', "exclude-markdown", Required = false, HelpText = "Exclude markdown input from parsed result if present.")]
		public bool ExcludeMarkdown { get; set; }

		[Option('i', "full-index", Required = false, HelpText = "Include the section names the tests' file names.")]
		public bool FullIndex { get; set; }

		[Option('O', "output", Required = true, HelpText = "Output file.")]
		public string? Output { get; set; }
	}
}