using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace MarkXLibrary
{
	public static class Transformer
	{
		private static string XsltResourceName { get; } = "MarkXLibrary.mapping.xslt";
		private static XslCompiledTransform Xslt { get; } = new XslCompiledTransform();

		static Transformer()
		{
			LoadTransformation();
		}

		public static void LoadTransformation()
		{
			var assembly = Assembly.GetExecutingAssembly();

			var xsltInput = "";
			using (Stream? stream = assembly.GetManifestResourceStream(XsltResourceName))
			{
				if (stream != null)
				{
					using StreamReader sr = new(stream);
					xsltInput = sr.ReadToEnd();
				}
				else
				{
					throw new FileNotFoundException("Failed to load the XSLT file.");
				}
			}

			XsltSettings xsltSettings = new()
			{
				EnableScript = true
			};

			using var srt = new StringReader(xsltInput);
			using XmlReader xrt = XmlReader.Create(srt, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore });
			Xslt.Load(xrt, xsltSettings, new XmlUrlResolver());
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
					if (startLines.Length == 2 && startLines.All(x => string.IsNullOrWhiteSpace(x)))
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
			return (own != null && provided != null) ?
				(preferOwnResult ? own : provided) : (own ?? provided);
		}

		public static string? TransformXml(string xml, bool indentCode, IEnumerable<string> extensionList)
		{
			var output = "";

			XsltExtension xsltExtension = new();
			XsltArgumentList xsltArguments = new();
			xsltArguments.AddExtensionObject("mark:ext", xsltExtension);
			xsltArguments.AddParam("indented-code", "", indentCode);
			xsltArguments.AddParam("extensions", "", string.Join(" ", extensionList));

			using (StringReader stringReader = new(xml))
			{
				using XmlTextReader xmlReader = new(stringReader);
				xmlReader.XmlResolver = null;
				using StringWriter stringWriter = new();
				using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, Xslt.OutputSettings);
				Xslt.Transform(xmlReader, xsltArguments, xmlWriter);
				output = stringWriter.ToString();
			}
			return output.ReplaceLineEndings("\n");
		}
	}
}
