using CommandLine;

namespace MarkX.ConsoleUI
{
	[Verb("check", HelpText = "Check file.")]
	public class CheckOptions : Options
	{
		[Option('R', "result", Required = false, HelpText = "Expected result of parsing.")]
		public string? Result { get; set; }

		[Option('r', "own-result", Required = false, HelpText = "Primarily use the shared expected result file.")]
		public bool OwnResult { get; set; }
	}
}