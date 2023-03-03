using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace MarkXLibrary
{
	public static class Together
	{
		private static string transformPath { get; } = @"C:\Users\andre\source\repos\MarkX\MarkXLibrary\mapping.xslt";
		private static XslCompiledTransform xslt { get; } = new XslCompiledTransform();

		static Together()
        {
			LoadTransformation();
		}
		public static void LoadTransformation()
        {
			if (transformPath == null || !File.Exists(transformPath))
			{
				return;
			}

			var xsltInput = "";
			using (StreamReader sr = new StreamReader(transformPath))
			{
				xsltInput = sr.ReadToEnd();
			}

			using (var srt = new StringReader(xsltInput))
			{
				using (XmlReader xrt = XmlReader.Create(srt, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
				{
					xslt.Load(xrt);
				}
			}
		}
		// TODO classes
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

		public static string? ParseXml(string xml)
		{
			string? result = null;
			XElement? root;
			try
			{
				root = XElement.Parse(xml, LoadOptions.PreserveWhitespace);
			}
			catch (Exception)
			{
				return result;
			}

			if (root != null)
			{
				var parentInheritance = new InheritanceData()
				{
					IncludeText = false,
					Separate = false,
					Parenthesise = false,
				};
				var (lines, _) = XMLParser.Parse(root, parentInheritance);
				if (lines != null)
				{
					result = string.Join("\n", lines);
					result += "\n";
				}
			}
			return result;
		}

		public static void DisableElement(string name)
		{
			var codeBlockElement = Mapping.DefaultMappingSpecification?.GetMappingDefinitionById(name);
			if (codeBlockElement != null)
			{
				codeBlockElement.IsEnabled = false;
			}
		}

		// EXP
		public static string? TransformXml(string xml)
		{
			var output = "";

			using (StringReader stringReader = new StringReader(xml))
			{
				using (XmlReader xmlReader = XmlReader.Create(stringReader))
				{
					using (StringWriter stringWriter = new StringWriter())
					{
						using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xslt.OutputSettings))
						{
							xslt.Transform(xmlReader, xmlWriter);
							output = stringWriter.ToString();
						}
					}
				}
			}

			Console.WriteLine(output);
			return output;
		}
	}
}
