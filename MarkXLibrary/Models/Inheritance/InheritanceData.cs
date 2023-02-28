using System.Collections.Generic;

namespace MarkXLibrary
{
	public class InheritanceData
	{
		public bool IncludeText { get; set; }
		public bool Separate { get; set; }
		public bool Parenthesise { get; set; }
		public List<InheritanceInfo> Inheritances { get; set; } = new List<InheritanceInfo>() { };
	}
}