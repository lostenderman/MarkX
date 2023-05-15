using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarkXConsole
{
    public static class InfoWriter
    {
        public static void PrintResults(List<SectionFile>? inputFiles, bool includeTests)
        {
            if (inputFiles == null)
            {
                return;
            }
            var output = new Info
            {
                InvalidFiles = inputFiles.Where(x => x.FileType == FileType.Invalid).ToList(),
                InvalidXMLs = GetInvalidXMLs(inputFiles),
                Summary = GetSummary(inputFiles)
            };

            if (includeTests)
            {
                output.FailingTests = GetFailingTests(inputFiles);
            }

            Console.WriteLine(Serialize(output));
        }

        private static string Serialize(Info outputInfo)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(outputInfo, jsonOptions);
        }

        private static List<SectionFile> GetFiles(List<SectionFile> inputFiles,
            Func<SectionFile, bool> inputFileFilter,
            Func<Section, bool> sectionFilter,
            Func<Test, bool> testFilter)
        {
            var files = new List<SectionFile>();

            foreach (var inputFile in inputFiles)
            {
                if (!inputFileFilter(inputFile))
                {
                    continue;
                }

                var updatedFile = new SectionFile
                {
                    Name = inputFile.Name
                };
                files.Add(updatedFile);

                foreach (var section in inputFile.Sections)
                {
                    if (!sectionFilter(section))
                    {
                        continue;
                    }

                    var sec = new Section
                    {
                        Name = section.Name
                    };
                    updatedFile.Sections.Add(sec);
                    sec.Tests = section.Tests.Where(x => testFilter(x)).ToList();
                }
            }

            return files;
        }


        private static List<SectionFile> GetInvalidXMLs(List<SectionFile> inputFiles)
        {
            return GetFiles(inputFiles,
                inputFile => inputFile.FileType != FileType.Invalid && !inputFile.AllTestsAreValid,
                section => !section.AllTestsAreValid,
                test => !test.IsValid);
        }

        private static List<SectionFile> GetFailingTests(List<SectionFile> inputFiles)
        {
            return GetFiles(inputFiles,
                inputFile => !inputFile.AllValidTestsPass,
                section => !section.AllValidTestsPass,
                test => !test.IsPassing && test.IsValid);
        }

        private static Summary GetSummary(List<SectionFile> inputFiles)
        {
            var tests = inputFiles
                .Where(x => x.FileType != FileType.Invalid)
                .SelectMany(x => x.Sections)
                .SelectMany(x => x.Tests);

            var summary = new Summary
            {
                All = tests.Count(),
                Passed = tests.Count(x => x.IsPassing),
                Failed = tests.Count(x => !x.IsPassing),
                Included = tests.Count(x => x.IsValid),
                Skipped = tests.Count(x => !x.IsValid)
            };
            return summary;
        }
    }
}