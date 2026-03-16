using System;
using System.Collections.Generic;
using EXERCISE.Loaders;

namespace EXERCISE.Model
{
    [Serializable]
    public class DocumentExerciseData : ExerciseData
    {
        public List<ParagraphJson> paragraphs;
        public RegionsJson regions;
    }
}