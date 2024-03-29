﻿using MarkXLibrary;

namespace MarkXConsole
{
    public static class TestParser
    {
        public static void Run(ParseOptions options)
        {
            List<SectionFile>? inputFiles = Reader.LoadInputFiles(options.Input, Settings.InputDirectoryNestingLevel);

            NormalizeSectionNames(options, inputFiles);
            TryParseTests(inputFiles, options);

            if (!options.Quiet)
            {
                InfoWriter.PrintResults(inputFiles, false);
            }
            ExportParsedTests(options, inputFiles);
        }

        public static void ExportParsedTests(ParseOptions options, List<SectionFile>? inputFiles)
        {
            if (options.Output == null)
            {
                return;
            }

            if (options.UngroupSections)
            {
                ResetSectionNames(inputFiles);
            }

            var allSections = GroupSections(inputFiles);
            var allValidTests = allSections.SelectMany(x => x.Tests).Where(x => x.IsValid).ToList();

            if (allValidTests.Count == 1 && options.ParseToFile)
            {
                Writer.ExportTest(options, allValidTests.First(), options.Output);
                return;
            }

            if (!options.ParseToTree)
            {
                Writer.ExportJson(allSections, options);
                return;
            }

            Writer.ExportToTree(allSections, options);
        }

        public static void ResetSectionNames(List<SectionFile>? inputFiles)
        {
            if (inputFiles == null)
            {
                return;
            }
            var sections = inputFiles
                .SelectMany(x => x.Sections);

            foreach (var section in sections)
            {
                section.Name = "";
            }
        }

        public static void NormalizeSectionNames(Options options, List<SectionFile>? inputFiles)
        {
            if (inputFiles == null)
            {
                return;
            }
            char[] delims = new char[] { ' ', '\t' };
            foreach (var inputFile in inputFiles)
            {
                if (inputFile.Sections == null)
                {
                    continue;
                }
                foreach (var section in inputFile.Sections)
                {
                    var foldedWhitespaces = section.Name?.ToLower().Split(delims).Where(s => !string.IsNullOrWhiteSpace(s));
                    var normalizedName = "";
                    if (foldedWhitespaces != null)
                    {
                        normalizedName = string.Join("_", foldedWhitespaces);
                    }
                    if (options.IsolateSections)
                    {
                        normalizedName = string.Join('-', inputFile.FileInfo?.Name, normalizedName);
                    }
                    section.Name = normalizedName;
                }
            }
        }

        public static void TryParseTests(List<SectionFile>? inputFiles, Options options)
        {
            if (inputFiles == null)
            {
                return;
            }

            foreach (var inputFile in inputFiles)
            {
                foreach (var section in inputFile.Sections)
                {
                    foreach (var test in section.Tests)
                    {
                        if (test.XML == null)
                        {
                            continue;
                        }

                        string? parsed = null;
                        try
                        {
                            parsed = Transformer.TransformXml(test.XML, options?.IndentCode ?? false, options?.Extensions ?? new List<string>());
                            if (inputFile.FileType == FileType.PossiblyXML)
                            {
                                inputFile.FileType = FileType.XML;
                            }
                            test.IsValid = true;
                        }
                        catch (System.Xml.XmlException)
                        {
                            if (inputFile.FileType == FileType.PossiblyXML)
                            {
                                inputFile.FileType = FileType.Invalid;
                            }
                            test.IsValid = false;
                        }

                        if (parsed == null)
                        {
                            if (inputFile.FileType == FileType.PossiblyXML)
                            {
                                inputFile.FileType = FileType.Invalid;
                            }
                            continue;
                        }

                        test.Output = parsed;
                    }
                    section.UpdatePassingStatus();
                    section.UpdateValidityStatus();
                }
                inputFile.UpdatePassingStatus();
                inputFile.UpdateValidityStatus();
            }
        }

        public static List<Section> GroupSections(List<SectionFile>? inputFiles)
        {
            var groupedSections = inputFiles?
                .SelectMany(x => x.Sections)
                .GroupBy(x => x.Name)
                .Select(x => new Section()
                {
                    Name = x.Key,
                    Tests = x.SelectMany(x => x.Tests).ToList()
                })
                .ToList();

            return groupedSections ?? new List<Section>();
        }
    }
}
