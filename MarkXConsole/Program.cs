using CommandLine;
using CommandLine.Text;
using MarkX.ConsoleUI.Runners;
using MarkX.CommandParser;

namespace MarkX.ConsoleUI
{
	class Program
	{
		static int Main(string[] args)
		{
			var parser = new Parser(with => with.HelpWriter = null);
			var parserResult = parser.ParseArguments<ParseOptions, CheckOptions>(args);
			parserResult
				.WithParsed<ParseOptions>(options => TestParser.Run(options))
				.WithParsed<CheckOptions>(options => TestChecker.Run(options))
				.WithNotParsed(errs => DisplayHelp(parserResult));
			return 0;
		}

		static int DisplayHelp(ParserResult<object> parserResult)
		{
			Console.WriteLine(HelpText.AutoBuild(parserResult, h => {
				h.Heading = "MarkX 1.0.0";
				h.OptionComparison = Comparator.RequiredThenAlphaShortComparison;
				return h;
			}));
			return 1;
		}
	}
}
