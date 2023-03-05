using System;

namespace MarkXLibrary
{
	public class FallCondition
	{
		public string? Key { get; set; }
		public FallConditionType? Type { get; set; }
		public Func<string, bool>? Condition { get; set; }
	}
}