using MarkXLibrary;

namespace MarkXConsoleUI
{
	public static class Writer
	{
		public static void WriteToTree(List<Section> sections, ParseOptions options)
		{
			if (options.Output == null)
			{
				return;
			}
			var treeOutputDirectory = Directory.CreateDirectory(options.Output);
			foreach (var section in sections)
			{
				if (section.Tests == null || !section.Tests.Any())
				{
					continue;
				}
				var sectionName = section.Name ?? "";
				var sectionDirectory = Directory.CreateDirectory(Path.Combine(treeOutputDirectory.FullName, sectionName));
				WriteTests(options, section, sectionDirectory.FullName);
			}
		}

		public static int WriteTests(ParseOptions options, Section section, string directoryName)
		{
			var fileIndex = 0;
			for (int i = 0; i < section.Tests.Count; i++)
			{
				var test = section.Tests[i];
				if (!test.IsValid)
				{
					continue;
				}
				var fileName = "";
				if (options.FullIndex && !string.IsNullOrWhiteSpace(section.Name))
				{
					fileName += section.Name + "_";
				}

				fileName += string.Format("{0:D3}", fileIndex + Settings.StartIndex);
				var fullPath = Path.Combine(directoryName, fileName) + Settings.OutputFileExtension;

				WriteTest(options, test, fullPath);
				fileIndex++;
			}
			return 0;
		}

		public static int WriteTest(ParseOptions options, Test test, string destination)
		{
			using (StreamWriter sw = new StreamWriter(destination))
			{
				if (test.Markdown != null && !options.ExcludeMarkdown)
				{
					sw.WriteLine(ResourceStrings.MarkdownInputStart);
					sw.Write(test.Markdown);
					sw.WriteLine(ResourceStrings.MarkdownInputEnd);
				}

				sw.Write(string.Join("\n", test.Output));
			}
			return 0;
		}
	}
}