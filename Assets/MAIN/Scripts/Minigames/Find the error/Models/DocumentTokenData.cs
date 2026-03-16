using System;

namespace EXERCISE.Model
{
    public enum DocumentGranularity { Clause, Paragraph, Sentence }

    [Serializable]
    public class DocumentTokenData : TokenData
    {
        public string documentId;
        public DocumentGranularity granularity;

        // Para orden estable
        public int orderIndex;
    }
}