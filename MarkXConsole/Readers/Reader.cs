using System.Text.Json;

namespace MarkXConsole
{
    public static class Reader
    {
        public static List<SectionFile>? LoadInputFiles(IEnumerable<string>? inputFilePaths, int inputDirectoryNestingLevel)
        {
            if (inputFilePaths == null || !inputFilePaths.Any())
            {
                return null;
            }

            List<SectionFile> inputFiles = new();

            foreach (var inputFilePath in inputFilePaths)
            {
                var file = LoadFile(inputFilePath, FileType.PossiblyJSON);
                if (file == null)
                {
                    if (inputDirectoryNestingLevel > 0 && Directory.Exists(inputFilePath))
                    {
                        var directoryFilePaths = Directory.GetFiles(inputFilePath);
                        var subFiles = LoadInputFiles(directoryFilePaths, inputDirectoryNestingLevel - 1);
                        if (subFiles != null)
                        {
                            inputFiles.AddRange(subFiles);
                        }
                    }
                    else
                    {
                        file = new SectionFile()
                        {
                            FileType = FileType.Invalid
                        };
                        inputFiles.Add(file);
                    }
                }
                else
                {
                    inputFiles.Add(file);
                }
            }

            return inputFiles;
        }

        public static SectionFile? LoadFile(string? path, FileType fileStartType)
        {
            if (path == null || !File.Exists(path))
            {
                return null;
            }

            SectionFile file = new()
            {
                FileInfo = new FileInfo(path)
            };
            file.Name = file.FileInfo.Name;
            using (StreamReader sr = file.FileInfo.OpenText())
            {
                file.RawContent = sr.ReadToEnd();
                if (fileStartType == FileType.PossiblyJSON)
                {
                    LoadFileStructure(file);
                }
            }
            return file;
        }

        public static void LoadFileStructure(SectionFile inputFile)
        {
            if (inputFile.IsEmpty())
            {
                inputFile.FileType = FileType.Invalid;
                return;
            }
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var structuredContent = JsonSerializer.Deserialize<List<Section>>(inputFile.RawContent!, jsonOptions);
                if (structuredContent != null)
                {
                    inputFile.Sections = structuredContent;
                }
                inputFile.FileType = FileType.JSON;
            }
            catch (JsonException)
            {
                inputFile.Sections.Add
                (
                    new Section()
                    {
                        Name = "",
                        Tests = new List<Test>()
                        {
                            new Test()
                            {
                                XML = inputFile.RawContent
                            }
                        }
                    }
                );
                inputFile.FileType = FileType.PossiblyXML;
            }
        }
    }
}
