using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace MarkXLibrary
{
	public class XsltExtension
	{
		public static string Hash(string text)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(text);
				byte[] hashBytes = md5.ComputeHash(inputBytes);
				return Convert.ToHexString(hashBytes).ToLower();
			}
		}

		public static string UnescapeUri(string text)
		{
			return Uri.UnescapeDataString(text);
		}
	}

	public static class Together
	{
		private static string TransformPath { get; } = @"C:\Users\andre\source\repos\MarkX\MarkXLibrary\mapping.xslt";
		private static XslCompiledTransform Xslt { get; } = new XslCompiledTransform();

		static Together()
		{
			LoadTransformation();
		}
		public static void LoadTransformation()
		{
			if (TransformPath == null || !File.Exists(TransformPath))
			{
				return;
			}

			var xsltInput = "";
			using (StreamReader sr = new(TransformPath))
			{
				xsltInput = sr.ReadToEnd();
			}

            XsltSettings xsltSettings = new()
            {
                EnableScript = true
            };

            using (var srt = new StringReader(xsltInput))
			{
				using (XmlReader xrt = XmlReader.Create(srt, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore }))
				{
					Xslt.Load(xrt, xsltSettings, new XmlUrlResolver());
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
			return (own != null && provided != null) ? 
				(preferOwnResult ? own : provided) : (own ?? provided);
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
		public static string? TransformXml(string xml, bool indentCode, IEnumerable<string> extensionList)
		{
			var output = "";

			XsltExtension xsltExtension = new XsltExtension();
			XsltArgumentList xsltArguments = new XsltArgumentList();
			xsltArguments.AddExtensionObject("mark:ext", xsltExtension);
			xsltArguments.AddParam("indented-code", "", indentCode);
			xsltArguments.AddParam("extensions", "", string.Join(" ", extensionList));

			using (StringReader stringReader = new StringReader(xml))
			{
				using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
				{
					xmlReader.XmlResolver = null;
					using (StringWriter stringWriter = new StringWriter())
					{
						using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, Xslt.OutputSettings))
						{
							Xslt.Transform(xmlReader, xsltArguments, xmlWriter);
							output = stringWriter.ToString();
						}
					}
				}
			}

			return output;
		}
	}
}
