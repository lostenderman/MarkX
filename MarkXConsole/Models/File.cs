using System.Text.Json.Serialization;

namespace MarkX.Core
{
	public class SectionFile
	{
		public FileInfo? FileInfo { get; set; }
		public FileType FileType { get; set; }
		public string? RawContent { get; set; }
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