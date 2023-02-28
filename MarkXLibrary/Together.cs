using System.Xml.Linq;

namespace MarkXLibrary
{
    public static class Together
    {
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
							var (lines, resetSeparation) = XMLParser.Parse(root, parentInheritance);
							if (lines != null)
							{
								test.Output = string.Join("\n", lines);
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
					if (startLines.Length == 2 && startLines.All(x => string.IsNullOrWhiteSpace(x)))
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
				test.Expected = ChooseExpectedResult(test.Expected, resultFile.RawContent, preferOwnResult);
			}
		}

		public static string? ChooseExpectedResult(string? own, string? provided, bool preferOwnResult)
		{
			if (own == null)
			{
				return provided;
			}
			if (provided == null)
			{
				return own;
			}
			if (preferOwnResult)
			{
				return own;
			}
			return provided;
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
