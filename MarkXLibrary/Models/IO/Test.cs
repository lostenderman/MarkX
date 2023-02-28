using System.Text.Json.Serialization;

namespace MarkXLibrary
{
	public class Test
	{
		public string? Markdown { get; set; }
		public string? XML { get; set; }
		public string? Expected { get; set; }
		public string? Output { get; set; }
		public int? Example { get; set; }
		public string? Note { get; set; }

		[JsonIgnore]
		public bool IsPassing { get; set; }

		[JsonIgnore]
		public bool IsValid { get; set; }
	}
}