using MarkX.Core;
using System.Collections.Generic;
using Xunit;

namespace MarkXTesting
{
    public class ParsingTests
    {
        private readonly string xmlHeader = "<?xml version =\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE document SYSTEM \"CommonMark.dtd\">\n\n";
        private readonly string xmlDocumentStart = "<document xmlns=\"http://commonmark.org/xml/1.0\">\n";
        private readonly string xmlDocumentEnd = "</document>\n";

        [Fact]
        public void Test1()
        {
            var xmlContent = "<paragraph>\n    <text>-1. </text>\n    <emph>\n      <text>not ok</text>\n    </emph>\n  </paragraph>\n";
            var indentCode = false;
            var extensionList = new List<string>();

            var xml = xmlHeader + xmlDocumentStart + xmlContent + xmlDocumentEnd;
            var expectedResult = Together.TransformXml(xml, indentCode, extensionList);
            var actualResult = "documentBegin\nemphasis: not ok\ndocumentEnd\n";

            var result = expectedResult == actualResult;
            Assert.Equal(expectedResult, actualResult);
        }
    }
}