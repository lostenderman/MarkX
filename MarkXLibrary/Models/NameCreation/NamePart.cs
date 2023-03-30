using System.Collections.Generic;

namespace MarkX.Core
{
	public class NamePart
	{
		public NamePartType Type { get; set; }
		public string? Value { get; set; }
		public bool IncludeAtStart { get; set; } = true;
		public bool IncludeAtEnd { get; set; } = true;
		public List<NamePart>? SubNameParts { get; set; }
	}
}