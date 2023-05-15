using System.Text.Json.Serialization;

namespace MarkXConsole
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
            AllValidTestsPass = Tests.All(x => !x.IsValid || x.IsPassing);
        }

        public void UpdateValidityStatus()
        {
            AllTestsAreValid = Tests.All(x => x.IsValid == true);
        }
    }
}