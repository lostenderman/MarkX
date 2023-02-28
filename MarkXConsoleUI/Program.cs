using CommandLine;
using MarkXConsoleUI;
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

		// UI
		public static void RunParsing(ParseOptions options)
		{
			List<SectionFile>? inputFiles = FileLoader.LoadInputFiles(options.Input, Settings.InputDirectoryNestingLevel);

			NormalizeSectionNames(options, inputFiles);

			PrepareSpecification(options);
			Together.TryParseTests(inputFiles);

			if (!options.Quiet)
			{
				InfoWriter.PrintParsingResults(options, inputFiles);
			}
			ExportParsedTests(options, inputFiles);
		}

		// UI
		public static void RunChecking(CheckOptions options)
		{
			List<SectionFile>? inputFiles = FileLoader.LoadInputFiles(options.Input, Settings.InputDirectoryNestingLevel);
			SectionFile? resultFile = FileLoader.LoadFile(options.Result, FileType.Unstructured);

			NormalizeSectionNames(options, inputFiles);

			PrepareSpecification(options);
			Together.AssignExpectedResult(inputFiles, resultFile, options.OwnResult);

			Together.TryParseTests(inputFiles);

			if (!options.Quiet)
			{
				InfoWriter.PrintCheckingResults(options, inputFiles);
			}
		}

		// UI
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

			var allSections = Together.GroupSections(inputFiles);
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

		// LIB
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

		// UI
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

		// LIB
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
		
		// LIB
		public static void PrepareSpecification(Options options)
		{
			Mapping.ApplyExtensions(options.Extensions);

			if (options.IndentCode)
			{
				var codeBlockElement = Mapping.DefaultMappingSpecification?.GetMappingDefinitionById("fenced_code_block");
				if (codeBlockElement != null)
				{
					codeBlockElement.IsEnabled = false;
				}
			}
		}
	}
}
