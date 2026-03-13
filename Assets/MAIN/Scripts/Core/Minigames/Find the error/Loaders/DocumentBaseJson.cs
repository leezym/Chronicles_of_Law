using System;
using System.Collections.Generic;

namespace EXERCISE.Loaders
{
    [Serializable]
    public class DocumentBaseJson
    {
        public string exerciseId;
        public string title;
        public string type; // "legal_document"
        public List<ParagraphJson> paragraphs;
    }

    [Serializable]
    public class ParagraphJson
    {
        public string pid;     // "P0"
        public string align;   // left|center|right|justify
        public List<RunJson> runs;
        public string plain;   // texto plano para offsets
    }

    [Serializable]
    public class RunJson
    {
        public string text;
        public bool bold;
        public bool italic;
        public bool underline;
    }

    [Serializable]
    public class RegionsJson
    {
        public float pointsPerCorrect = 1f;
        public List<RegionJson> regions;
    }

    [Serializable]
    public class RegionJson
    {
        public string id;
        public bool enabled;
        public bool correct;
        public float points;
        public List<SpanJson> spans;
    }

    [Serializable]
    public class SpanJson
    {
        public string pid;
        public int start;
        public int end;
    }
}