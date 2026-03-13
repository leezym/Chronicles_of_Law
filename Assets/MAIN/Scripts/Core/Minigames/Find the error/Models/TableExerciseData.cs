using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXERCISE.Model
{
    [Serializable]
    public class TableExerciseData : ExerciseData
    {
        public List<TableTokenData> tableTokens;
        public List<DocumentTokenData> documentTokens;
        public List<TableImageData> tableImages;
    }
}