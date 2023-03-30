namespace MarkX.Core
{
	public class InheritanceInfo : ICloneable
	{
		public int MaxLevel { get; set; } = 1;
		public int CurrentLevel { get; set; }

		public int Iteration { get; set; }
		public string? Key { get; set; }
		public string? CurrentValue { get; set; }
		public string? ParentElement { get; set; }
		public List<string>? ChildElements { get; set; }
		public Func<string, string>? Logic { get; set; }
		public string? ValueInitOverride { get; set; }

		public bool IsPassable() => MaxLevel > 0 && CurrentLevel < MaxLevel;
		public bool IsActivated() => CurrentLevel > 0;

		public object Clone()
		{
			InheritanceInfo other = (InheritanceInfo)this.MemberwiseClone();
			return other;
		}
	}
}