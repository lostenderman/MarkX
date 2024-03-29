﻿namespace MarkXConsole
{
    public class Info
    {
        public Summary? Summary { get; set; }
        public List<SectionFile> InvalidFiles { get; set; } = new List<SectionFile>();
        public List<SectionFile> InvalidXMLs { get; set; } = new List<SectionFile>();
        public List<SectionFile> FailingTests { get; set; } = new List<SectionFile>();
    }
}
