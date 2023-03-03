using CommandLine;
using MarkXLibrary;
using System.Text.Json;

namespace MarkXConsoleUI
{
	class Program
	{
		static int Main(string[] args)
		{
			Parser.Default.ParseArguments<ParseOptions, CheckOptions>(args)
				.WithParsed<ParseOptions>(options => RunParsing(options))
				.WithParsed<CheckOptions>(options => RunChecking(options))
				.WithNotParsed(errors => ShowErrors(errors));
			return 0;
		}

		public static int ShowErrors(IEnumerable<Error> errors)
		{
			return 0;
		}

		public static void RunParsing(ParseOptions options)
		{
			List<SectionFile>? inputFiles = FileLoader.LoadInputFiles(options.Input, Settings.InputDirectoryNestingLevel);

			NormalizeSectionNames(options, inputFiles);

			Mapping.ApplyExtensions(options.Extensions);

			if (options.IndentCode)
			{
				Together.DisableElement("fenced_code_block");
			}
			TryParseTests(inputFiles);

			if (!options.Quiet)
			{
				InfoWriter.PrintParsingResults(options, inputFiles);
			}
			ExportParsedTests(options, inputFiles);
		}

		public static void RunChecking(CheckOptions options)
		{
			List<SectionFile>? inputFiles = FileLoader.LoadInputFiles(options.Input, Settings.InputDirectoryNestingLevel);
			SectionFile? resultFile = FileLoader.LoadFile(options.Result, FileType.Unstructured);

			NormalizeSectionNames(options, inputFiles);

			Mapping.ApplyExtensions(options.Extensions);

			if (options.IndentCode)
			{
				Together.DisableElement("fenced_code_block");
			}
			AssignExpectedResult(inputFiles, resultFile, options.OwnResult);

			TryParseTests(inputFiles);
			CheckTests(inputFiles);

			if (!options.Quiet)
			{
				InfoWriter.PrintCheckingResults(options, inputFiles);
			}
		}

		public static void ExportParsedTests(ParseOptions options, List<SectionFile>? inputFiles)
		{
			if (options.Output == null)
			{
				return;
			}

			if (!options.GroupSections)
			{
				ResetSectionNames(inputFiles);
			}

			var allSections = GroupSections(inputFiles);
			var allValidTests = allSections.SelectMany(x => x.Tests).Where(x => x.IsValid).ToList();

			if (allValidTests.Count() == 1 && options.ParseToFile)
			{
				Writer.WriteTest(options, allValidTests.First(), options.Output);
				return;
			}

			if (!options.ParseToTree)
			{
				CreateJson(allSections, options);
				return;
			}

			Writer.WriteToTree(allSections, options);
		}

		public static void ResetSectionNames(List<SectionFile>? inputFiles)
		{
			if (inputFiles == null)
			{
				return;
			}
			var sections = inputFiles
				.SelectMany(x => x.Sections);

			foreach (var section in sections)
			{
				section.Name = "";
			}
		}

		public static void CreateJson(List<Section> sections, ParseOptions options)
		{
			if (options.Output == null)
			{
				return;
			}
			string? serialized = JsonSerializer.Serialize(sections);
			using (StreamWriter sw = new(options.Output))
			{
				sw.WriteLine(serialized);
			}
		}

		public static void NormalizeSectionNames(Options options, List<SectionFile>? inputFiles)
		{
			if (inputFiles == null)
			{
				return;
			}
			char[] delims = new char[] { ' ', '\t' };
			foreach (var inputFile in inputFiles)
			{
				if (inputFile.Sections == null)
				{
					continue;
				}
				foreach (var section in inputFile.Sections)
				{
					var foldedWhitespaces = section.Name?.ToLower().Split(delims).Where(s => !string.IsNullOrWhiteSpace(s));
					var normalizedName = "";
					if (foldedWhitespaces != null)
					{
						normalizedName = string.Join("_", foldedWhitespaces);
					}
					if (options.IsolateSections)
					{
						normalizedName = string.Join('-', inputFile.FileInfo?.Name, normalizedName);
					}
					section.Name = normalizedName;
				}
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
				test.Expected = Together.ChooseExpectedResult(test.Expected, resultFile.RawContent, preferOwnResult);
			}
		}

		public static void TryParseTests(List<SectionFile>? inputFiles)
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
						if (test.XML == null)
						{
							continue;
						}

						var parsed = Together.ParseXml(test.XML);
						var parsed2 = Together.TransformXml(test.XML);

						Console.WriteLine(parsed2);
						if (parsed == null)
						{
							if (inputFile.FileType == FileType.PossiblyXML)
							{
								inputFile.FileType = FileType.Invalid;
							}
							test.IsValid = false;
						}
						else
						{
							if (inputFile.FileType == FileType.PossiblyXML)
							{
								inputFile.FileType = FileType.XML;
							}
							test.IsValid = true;
						}
						test.Output = parsed;
					}
				}
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
						test.IsPassing = Together.CompareResults(test.Output, test.Expected);
					}

					section.UpdatePassingStatus();
					section.UpdateValidityStatus();
				}

				inputFile.UpdatePassingStatus();
				inputFile.UpdateValidityStatus();
			}
		}

		public static List<Section> GroupSections(List<SectionFile>? inputFiles)
		{
			var groupedSections = inputFiles?
				.SelectMany(x => x.Sections)
				.GroupBy(x => x.Name)
				.Select(x => new Section()
				{
					Name = x.Key,
					Tests = x.SelectMany(x => x.Tests).ToList()
				})
				.ToList();

			return groupedSections ?? new List<Section>();
		}
	}
}
