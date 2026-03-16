using System;
using System.Collections.Generic;
using EXERCISE.Loaders;

namespace EXERCISE.Model
{
    public enum ExerciseType {tabla, documento}
    
    [Serializable]
    public class ExerciseData
    {
        public string exerciseId;
        public string title;

        public ExerciseType type;
        public string folder;
    }
}