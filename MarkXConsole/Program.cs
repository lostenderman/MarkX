using CommandLine;
using MarkX.ConsoleUI.Runners;

namespace MarkX.ConsoleUI
{
	class Program
	{
		static int Main(string[] args)
		{
			Parser.Default.ParseArguments<ParseOptions, CheckOptions>(args)
				.WithParsed<ParseOptions>(options => TestParser.Run(options))
				.WithParsed<CheckOptions>(options => TestChecker.Run(options))
				.WithNotParsed(errors => ShowErrors(errors));
			return 0;
		}

		public static int ShowErrors(IEnumerable<Error> errors)
		{
			return 0;
		}
	}
}
