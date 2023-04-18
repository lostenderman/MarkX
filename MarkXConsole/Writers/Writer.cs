using MarkX.Core;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarkX.ConsoleUI
{
	public static class Writer
	{
		public static void ExportToTree(List<Section> sections, ParseOptions options)
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
				ExportTests(options, section, sectionDirectory.FullName);
			}
		}

		public static int ExportTests(ParseOptions options, Section section, string directoryName)
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

				ExportTest(options, test, fullPath);
				fileIndex++;
			}
			return 0;
		}

		public static int ExportTest(ParseOptions options, Test test, string destination)
		{
			using StreamWriter sw = new(destination);
			if (test.Markdown != null && !options.ExcludeMarkdown)
			{
				sw.WriteLine(ResourceStrings.MarkdownInputStart);
				sw.Write(test.Markdown);
				sw.WriteLine(ResourceStrings.MarkdownInputEnd);
			}

			sw.Write(string.Join("\n", test.Output));
			return 0;
		}

		public static void ExportJson(List<Section> sections, ParseOptions options)
		{
			if (options.Output == null)
			{
				return;
			}
			var jsonOptions = new JsonSerializerOptions
			{
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};
			string? serialized = JsonSerializer.Serialize(sections, jsonOptions);
			using StreamWriter sw = new(options.Output);
			sw.WriteLine(serialized);
		}
	}
}