using CommandLine;

namespace MarkXConsoleUI
{
	public class Options
	{
		[Option('g', "group-sections", Required = false, HelpText = "Group tests to sections in the output.")]
		public bool GroupSections { get; set; }

		// LIB TOO
		[Option('c', "code-indented", Required = false, HelpText = "Render the default code element as indented code, rather than fenced code")]
		public bool IndentCode { get; set; }

		// LIB TOO
		[Option('e', "extensions", Required = false, HelpText = "Add custom extensions.")]
		public IEnumerable<string>? Extensions { get; set; }

		[Option('s', "isolate-sections", Required = false, HelpText = "Do not merge identically named sections in different files.")]
		public bool IsolateSections { get; set; }

		[Option('q', "quiet", Required = false, HelpText = "Suppress any output.")]
		public bool Quiet { get; set; }

		[Option('I', "input", Required = true, HelpText = "Files to be parsed.")]
		public IEnumerable<string>? Input { get; set; }
	}
}