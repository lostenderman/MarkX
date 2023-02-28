using CommandLine;
using MarkXLibrary;
using System.Text.Json;
using System.Xml.Linq;

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

		#region new
		public static List<SectionFile> InputFiles = new List<SectionFile>();

		public static SectionFile? ResultFile;

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

		public static void RunParsing(ParseOptions options)
		{
			LoadInputFiles(options);
			LoadFileContents(options);
			NormalizeSectionNames(options);

			PrepareSpecification(options);
			TryParseTests();

			if (!options.Quiet)
			{
				InfoWriter.PrintParsingResults(options);
			}
			ExportParsedTests(options);
		}

		public static void RunChecking(CheckOptions options)
		{
			LoadInputFiles(options);
			LoadResultFile(options);
			LoadFileContents(options);
			NormalizeSectionNames(options);

			PrepareSpecification(options);
			AssignExpectedResult(options);

			TryParseTests();

			if (!options.Quiet)
			{
				InfoWriter.PrintCheckingResults(options);
			}
		}

		public static void AssignExpectedResult(CheckOptions options)
		{
			if (ResultFile == null)
			{
				return;
			}
			var tests = InputFiles
				.SelectMany(x => x.Sections)
				.SelectMany(x => x.Tests);

			foreach (var test in tests)
			{
				test.Expected = ChooseExpectedResult(options, test.Expected, ResultFile.RawContent);
			}
		}

		public static string? ChooseExpectedResult(CheckOptions options, string? own, string? provided)
		{
			if (own == null)
			{
				return provided;
			}
			if (provided == null)
			{
				return own;
			}
			if (options.OwnResult)
			{
				return own;
			}
			return provided;
		}

		public static void LoadResultFile(CheckOptions options)
		{
			if (options.Result == null)
			{
				return;
			}
			SectionFile file = new SectionFile();

			file.FileInfo = new FileInfo(options.Result);
			using (StreamReader sr = file.FileInfo.OpenText())
			{
				var content = "";
				while ((content = sr.ReadLine()) != null)
				{
					file.RawContent += content + "\n";
				}
			}
			ResultFile = file;
		}

		public static void ExportParsedTests(ParseOptions options)
		{
			if (options.Output == null)
			{
				return;
			}

			if (!options.GroupSections)
			{
				ResetSectionNames(options);
			}

			var allSections = GroupSections(options);
			var allValidTests = allSections.SelectMany(x => x.Tests).Where(x => x.IsValid).ToList();

			if (allValidTests.Count() == 1 && options.ParseToFile)
			{
				Writer.WriteTest(options, allValidTests.First(), options.Output);
				return;
			}

			if (!options.ParseToTree)
			{
				CreateJson(allSections, allValidTests, options);
				return;
			}

			Writer.WriteToTree(allSections, options);
		}

		public static void ResetSectionNames(ParseOptions options)
		{
			var sections = InputFiles
				.SelectMany(x => x.Sections);

			foreach (var section in sections)
			{
				section.Name = "";
			}
		}

		public static void CreateJson(List<Section> sections, List<Test> tests, ParseOptions options)
		{
			if (options.Output == null)
			{
				return;
			}
			var serialized = "";
			serialized = JsonSerializer.Serialize<List<Section>>(sections);

			Console.WriteLine(sections.Count);

			using (StreamWriter sw = new StreamWriter(options.Output))
			{
				sw.WriteLine(serialized);
			}
		}

		public static List<Section> GroupSections(ParseOptions options)
		{
			var groupedSections = InputFiles
				.SelectMany(x => x.Sections)
				.GroupBy(x => x.Name)
				.Select(x => new Section()
				{
					Name = x.Key,
					Tests = x.SelectMany(x => x.Tests).ToList()
				})
				.ToList();

			return groupedSections;
		}

		public static void LoadInputFiles(Options options)
		{
			if (options.Input == null || !options.Input.Any())
			{
				return;
			}

			foreach (var path in options.Input)
			{
				Console.WriteLine(path);

				if (File.Exists(path))
				{
					LoadInputFile(path);
				}
				else if (Directory.Exists(path))
				{
					var directoryFilePaths = Directory.GetFiles(path);
					foreach (var filePath in directoryFilePaths)
					{
						LoadInputFile(filePath);
					}
				}
				else
				{
					SectionFile file = new SectionFile()
					{
						FileType = FileType.Invalid
					};
					InputFiles.Add(file);
				}
			}
		}

		public static void LoadInputFile(string path)
		{
			SectionFile file = new SectionFile()
			{
				FileInfo = new FileInfo(path),
				FileType = FileType.PossiblyJSON
			};
			using (StreamReader sr = file.FileInfo.OpenText())
			{
				var content = "";
				while ((content = sr.ReadLine()) != null)
				{
					file.RawContent += content;
				}
			}
			InputFiles.Add(file);
		}

		public static void NormalizeSectionNames(Options options)
		{
			char[] delims = new char[] { ' ', '\t' };
			foreach (var inputFile in InputFiles)
			{
				if (inputFile.Sections == null)
				{
					continue;
				}
				foreach (var section in inputFile.Sections)
				{
					var foldedWhitespaces = section.Name?.ToLower().Split(delims).Where(s => !String.IsNullOrWhiteSpace(s));
					var normalizedName = "";
					if (foldedWhitespaces != null)
					{
						normalizedName = String.Join("_", foldedWhitespaces);
					}
					if (options.IsolateSections)
					{
						normalizedName = string.Join('-', inputFile.FileInfo?.Name, normalizedName);
					}
					section.Name = normalizedName;
				}
			}
		}

		public static void LoadFileContents(Options options)
		{
			foreach (var inputFile in InputFiles)
			{
				if (inputFile.FileType == FileType.Invalid || inputFile.IsEmpty())
				{
					continue;
				}
				try
				{
					var jsonOptions = new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					};

					var structuredContent = JsonSerializer.Deserialize<List<Section>>(inputFile.RawContent!, jsonOptions);
					if (structuredContent != null)
					{
						inputFile.Sections = structuredContent;
					}
					inputFile.FileType = FileType.JSON;
				}
				catch (JsonException)
				{
					inputFile.Sections.Add
					(
						new Section()
						{
							Name = "",
							Tests = new List<Test>()
							{
								new Test()
								{
									XML = inputFile.RawContent
								}
							}
						}
					);
					inputFile.FileType = FileType.PossiblyXML;
				}
			}
		}

		public static void TryParseTests()
		{
			foreach (var inputFile in InputFiles)
			{
				foreach (var section in inputFile.Sections)
				{
					foreach (var test in section.Tests)
					{
						if (test.XML == null)
						{
							continue;
						}

						XElement? root = null;
						try
						{
							root = XElement.Parse(test.XML, LoadOptions.PreserveWhitespace);
							test.IsValid = true;
						}
						catch (Exception)
						{

						}

						if (root != null)
						{
							var parentInheritance = new InheritanceData()
							{
								IncludeText = false,
								Separate = false,
								Parenthesise = false,
							};
							var result = XMLParser.Parse(root, parentInheritance);
							if (result.lines != null)
							{
								test.Output = string.Join("\n", result.lines);
								test.Output += "\n";
							}
							test.IsPassing = CompareResults(test.Output, test.Expected);

							if (inputFile.FileType == FileType.PossiblyXML)
							{
								inputFile.FileType = FileType.XML;
							}
						}
						else
						{
							if (inputFile.FileType == FileType.PossiblyXML)
							{
								inputFile.FileType = FileType.Invalid;
							}
						}
					}

					section.UpdatePassingStatus();
					section.UpdateValidityStatus();
				}

				inputFile.UpdatePassingStatus();
				inputFile.UpdateValidityStatus();
			}
		}

		public static bool CompareResults(string? generated, string? expected)
		{
			if (generated == null || expected == null)
			{
				return false;
			}
			var expectedLines = expected.Split('\n');
			int markdownInputStartLineIndex = -1;
			int markdownInputEndLineIndex = -1;

			bool markdownStartFound = false;
			bool markdownEndFound = false;

			var index = 0;
			while (!markdownEndFound && index < expectedLines.Length)
			{
				string? line = expectedLines[index];
				if (!markdownStartFound)
				{
					var startLines = line.Split(ResourceStrings.MarkdownInputStart);
					if (startLines.Count() == 2 && startLines.All(x => string.IsNullOrWhiteSpace(x)))
					{
						markdownStartFound = true;
						markdownInputStartLineIndex = index;
					}
				}
				else if (!markdownEndFound)
				{
					var startLines = line.Split(ResourceStrings.MarkdownInputEnd);
					if (startLines.Count() == 2 && startLines.All(x => string.IsNullOrWhiteSpace(x)))
					{
						markdownEndFound = true;
						markdownInputEndLineIndex = index;
					}
				}
				index++;
			}
			var testResult = string.Join('\n', expectedLines, markdownInputEndLineIndex + 1, expectedLines.Length - markdownInputEndLineIndex - 1);
			return generated == testResult;
		}

		#endregion new
	}
}
