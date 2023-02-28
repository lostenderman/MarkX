namespace MarkXLibrary
{
    public static class ResourceStrings
    {
		public static string MarkdownInputStart { get; } = "<<<";
		public static string MarkdownInputEnd { get; } = ">>>";
		public static string BlockSeparator { get; } = "interblockSeparator";
		public static string BlockPositionStart { get; } = "Begin";
		public static string BlockPositionEnd { get; } = "End";
		public static string MultilinePositionStart { get; } = "BEGIN ";
		public static string MultilinePositionEnd { get; } = "END ";
		public static string MultilineAttributeStart { get; } = "- ";
		public static string InlineStart { get; } = ": ";
		public static string VerbatimDirectory { get; } = "./_markdown_test/";
		public static string Verbatim { get; } = ".verbatim";
		public static string HtmlCommentStart { get; } = "<!--";
		public static string HtmlCommentEnd { get; } = "-->";
	}
}
