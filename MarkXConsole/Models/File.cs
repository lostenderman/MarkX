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
			this.AllValidTestsPass = this.Sections.All(x => x.AllValidTestsPass == true);
		}

		public void UpdateValidityStatus()
		{
			this.AllTestsAreValid = this.Sections.All(x => x.AllTestsAreValid == true);
		}

		public bool IsEmpty()
		{
			return this.RawContent == null || this.RawContent == "";
		}
	}
}