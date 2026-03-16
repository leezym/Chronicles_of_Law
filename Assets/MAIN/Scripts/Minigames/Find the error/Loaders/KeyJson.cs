using System;
using System.Collections.Generic;

namespace EXERCISE.Loaders
{
    [Serializable]
    public class KeyJson
    {
        public int pointsPerCorrect = 1;
        public List<string> enabled;
        public List<string> correct;
    }
}