using MarkXLibrary;

namespace MarkXConsole
{
	public static class TestChecker
	{
		public static void Run(CheckOptions options)
		{
			List<SectionFile>? inputFiles = Reader.LoadInputFiles(options.Input, Settings.InputDirectoryNestingLevel);
			SectionFile? resultFile = Reader.LoadFile(options.Result, FileType.Unstructured);

			TestParser.NormalizeSectionNames(options, inputFiles);
			AssignExpectedResult(inputFiles, resultFile, options.OwnResult);

			TestParser.TryParseTests(inputFiles, options);
			CheckTests(inputFiles);

			if (!options.Quiet)
			{
				InfoWriter.PrintResults(inputFiles, true);
			}
		}

		public static void AssignExpectedResult(List<SectionFile>? inputFiles, SectionFile? resultFile, bool preferOwnResult)
		{
			if (inputFiles == null || resultFile == null)
			{
				return;
			}
			var tests = inputFiles
				.SelectMany(x => x.Sections)
				.SelectMany(x => x.Tests);

			foreach (var test in tests)
			{
				test.Expected = Transformer.ChooseExpectedResult(test.Expected, resultFile.RawContent, preferOwnResult);
			}
		}

		public static void CheckTests(List<SectionFile>? inputFiles)
		{
			if (inputFiles == null)
			{
				return;
			}
			foreach (var inputFile in inputFiles)
			{
				foreach (var section in inputFile.Sections)
				{
					foreach (var test in section.Tests)
					{
						test.IsPassing = Transformer.CompareResults(test.Output, test.Expected);
					}

					section.UpdatePassingStatus();
					section.UpdateValidityStatus();
				}

				inputFile.UpdatePassingStatus();
				inputFile.UpdateValidityStatus();
			}
		}
	}
}
