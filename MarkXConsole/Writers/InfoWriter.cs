using MarkX.Core;

namespace MarkX.ConsoleUI
{
	public static class InfoWriter
	{
		private static List<string> output = new();
		private static string IndentLevel1 { get; set; } = "    ";
		private static string IndentLevel2 { get; set; } = "        ";

		public static void PrintParsingResults(ParseOptions _, List<SectionFile>? inputFiles)
		{
			if (inputFiles == null)
			{
				return;
			}
			output = new List<string>();

			AddOpening();
			output.Add("--------------------------------\n");
			AddInvalidFiles(inputFiles);
			output.Add("--------------------------------\n");
			AddInvalidXMLs(inputFiles);
			output.Add("--------------------------------\n");
			AddSummary(false, inputFiles);

			Console.WriteLine(string.Join("\n", output));
		}

		public static void PrintCheckingResults(CheckOptions options, List<SectionFile>? inputFiles)
		{
			if (inputFiles == null)
			{
				return;
			}
			output = new List<string>();

			AddOpening();
			output.Add("--------------------------------\n");
			AddInvalidFiles(inputFiles);
			output.Add("--------------------------------\n");
			AddInvalidXMLs(inputFiles);
			output.Add("--------------------------------\n");
			AddResult(options, inputFiles);
			output.Add("--------------------------------\n");
			AddSummary(true, inputFiles);

			Console.WriteLine(string.Join("\n", output));
		}

		private static void AddOpening()
		{
			output.Add("# Results\n");
		}

		private static void AddInvalidFiles(List<SectionFile> inputFiles)
		{
			output.Add("## Invalid files\n");
			foreach (var inputFile in inputFiles)
			{
				if (inputFile.FileType != FileType.Invalid)
				{
					continue;
				}

				output.Add($"- {inputFile.FileInfo?.Name}\n");
			}
		}

		private static void AddInvalidXMLs(List<SectionFile> inputFiles)
		{
			output.Add("## Invalid xmls\n");
			foreach (var inputFile in inputFiles)
			{
				if (inputFile.FileType == FileType.Invalid || inputFile.AllTestsAreValid)
				{
					continue;
				}

				output.Add($"- {inputFile.FileInfo?.Name}\n");
				foreach (var section in inputFile.Sections)
				{
					if (section.AllTestsAreValid)
					{
						continue;
					}

					output.Add($"    - {section.Name}\n");
					for (int i = 0; i < section.Tests.Count; i++)
					{
						Test? test = section.Tests[i];
						if (test.IsValid)
						{
							continue;
						}

						var testLines = new List<String>();
						if (test.Example != null)
						{
							AddTestExample(testLines, test);
						}
						AddTestIndex(testLines, i);
						if (test.Note != null)
						{
							AddTestNote(testLines, test);
						}
						
						output.Add(IndentLines(testLines, IndentLevel2));
					}
				}

			}
		}

		private static void AddResult(CheckOptions options, List<SectionFile> inputFiles)
		{
			output.Add("## Failing tests\n");

			var testIndent = IndentLevel2;
			if (!options.GroupSections)
			{
				testIndent = IndentLevel1;
			}

			foreach (var inputFile in inputFiles)
			{
				if (inputFile.AllValidTestsPass)
				{
					continue;
				}

				output.Add($"- {inputFile.FileInfo?.Name}\n");
				int testIndex = 0;
				foreach (var section in inputFile.Sections)
				{
					if (section.AllValidTestsPass)
					{
						continue;
					}

					if (options.GroupSections)
					{
						output.Add($"    - {section.Name}\n");
						testIndex = 0;
					}
					
					for (int i = 0; i < section.Tests.Count; i++)
					{
						Test? test = section.Tests[i];
						if (test.IsPassing || !test.IsValid)
						{
							continue;
						}

						var delimeter = "";

						if (options.GroupSections)
						{
							delimeter = $"{i + 1}. ";
						}
						else
						{
							delimeter = $"{testIndex + 1}. ";
						}
						output.Add($"{testIndent}{delimeter}");
						AddTest(test, testIndent + new string(' ', delimeter.Length), i);
						testIndex++;
					}
				}
			}
		}

		private static void AddTest(Test test, string indent, int index)
		{
			var testLines = new List<String>();
			AddTestGeneratedResult(testLines, test);
			AddTestExpectedResult(testLines, test);

			if (test.Example != null)
			{
				AddTestExample(testLines, test);
			}
			AddTestIndex(testLines, index);
			if (test.Note != null)
			{
				AddTestNote(testLines, test);
			}

			output.Add(IndentLines(testLines, indent));
		}

		private static void AddTestGeneratedResult(List<String> lines, Test test)
		{
			lines.Add($"Generated\n");
			lines.Add($"```");
			if (test.Output != null)
			{
				lines.AddRange(test.Output.Split("\n"));
			}
			lines.Add($"```\n");
		}

		private static void AddTestExpectedResult(List<String> lines, Test test)
		{

			lines.Add($"Expected\n");
			lines.Add($"```");
			if (test.Expected != null)
			{
				lines.AddRange(test.Expected.Split("\n"));
			}
			lines.Add($"```\n");
		}

		private static void AddTestExample(List<String> lines, Test test)
		{
			lines.Add($"Example\n");
			lines.Add($"{test.Example}\n");
		}

		private static void AddTestNote(List<String> lines, Test test)
		{
			lines.Add($"Note\n");
			lines.Add($"{test.Note ?? ""}\n");
		}

		private static void AddTestIndex(List<String> lines, int index)
		{
			lines.Add($"Index\n");
			lines.Add($"{index}\n");
		}

		private static string IndentLines(IEnumerable<string> lines, string indent)
		{
			var joinedLines = string.Join("", lines.Select(x => indent + x + "\n"));
			return joinedLines ?? "";
		}
		
		private static void AddSummary(bool checking, List<SectionFile> inputFiles)
		{
			var tests = inputFiles
				.Where(x => x.FileType != FileType.Invalid)
				.SelectMany(x => x.Sections)
				.SelectMany(x => x.Tests);

			var l = tests.ToList();

			var all = tests.Count();
			var passed = tests.Count(x => x.IsPassing);
			var failed = tests.Count(x => !x.IsPassing);
			var included = tests.Count(x => x.IsValid);
			var skipped = tests.Count(x => !x.IsValid);

			output.Add("## Summary");
			output.Add("");
			output.Add($"Total - {all}  ");
			if (checking)
			{
				output.Add($"Passed - {passed}  ");
				output.Add($"Failed - {failed}  ");
			}
			output.Add($"Included - {included}  ");
			output.Add($"Skipped - {skipped}  ");
			return;
		}
	}
}