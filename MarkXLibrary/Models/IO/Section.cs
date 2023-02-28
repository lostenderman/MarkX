using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MarkXLibrary
{
	public class Section
	{
		public string? Name { get; set; }
		public List<Test> Tests { get; set; } = new List<Test>();

		[JsonIgnore]
		public bool AllValidTestsPass { get; set; }

		[JsonIgnore]
		public bool AllTestsAreValid { get; set; }

		public void UpdatePassingStatus()
		{
			this.AllValidTestsPass = this.Tests.All(x => !x.IsValid || x.IsPassing);
		}

		public void UpdateValidityStatus()
		{
			this.AllTestsAreValid = this.Tests.All(x => x.IsValid == true);
		}
	}
}