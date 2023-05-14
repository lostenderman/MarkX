using System.Text.Json.Serialization;

namespace MarkXConsole
{
	public class SectionFile
	{
		[JsonIgnore]
		public FileInfo? FileInfo { get; set; }

		[JsonIgnore]
		public FileType FileType { get; set; }

		[JsonIgnore]
		public string? RawContent { get; set; }
		public string? Name { get; set; }
		public List<Section> Sections { get; set; } = new List<Section>();

		[JsonIgnore]
		public bool AllValidTestsPass { get; set; }

		[JsonIgnore]
		public bool AllTestsAreValid { get; set; }

		public void UpdatePassingStatus()
		{
			AllValidTestsPass = Sections.All(x => x.AllValidTestsPass == true);
		}

		public void UpdateValidityStatus()
		{
			AllTestsAreValid = Sections.All(x => x.AllTestsAreValid == true);
		}

		public bool IsEmpty()
		{
			return RawContent == null || RawContent == "";
		}
	}
}