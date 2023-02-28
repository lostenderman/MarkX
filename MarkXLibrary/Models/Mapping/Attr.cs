using System.Collections.Generic;

namespace MarkXLibrary
{
	public class Attr
	{
		public string? MarkdownName { get; set; }
		public string? MarkupName { get; set; }
		public bool IsValueHashed { get; set; }
		public bool IsInputPercentEncoded { get; set; }
		public SpecialCharacterType SpecialCharacterMapping = SpecialCharacterType.Basic;
		public bool StripsEndingNewlineInHash { get; set; }
		public List<Pair>? Pairs { get; set; }
	}
}