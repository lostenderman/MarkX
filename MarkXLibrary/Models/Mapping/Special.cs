using System.Collections.Generic;

namespace MarkXLibrary
{
	public class SpecialCharacter
	{
		public string? MarkdownName { get; set; }
		public char? MarkupName { get; set; }
		public List<SpecialCharacterType> SpecialCharacterTypes { get; set; } = new List<SpecialCharacterType>();
	}

	public enum SpecialCharacterType
	{
		Basic,
		Uri,
		Minimal
	}
}