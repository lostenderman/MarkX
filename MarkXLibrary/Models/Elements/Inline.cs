using System;

namespace MarkX.Core
{
	public class Inline : Element
	{
		public bool IsValueHashed { get; set; }
		public bool StripEndingNewlineInHash { get; set; }
		public override bool Separate { get; set; } = false;
		public bool AllowsEmpty { get; set; } = true;
		public bool IncludeContent { get; set; } = true;
		public SpecialCharacterType SpecialCharacterMapping = SpecialCharacterType.Basic;
		public Func<string, string>? Format { get; set; }
	}
}