using System;
using System.Collections.Generic;
using EXERCISE.Model;

namespace EXERCISE.Loaders
{
    [Serializable]
    public class TableBaseJson
    {
        public string exerciseId;
        public string title;
        public ExerciseType type = ExerciseType.tabla;
        public string sheet;
        public DimensionJson dimension;
        public List<TableTokenJson> tokens;
        public List<TableImageJson> images;
    }

    [Serializable]
    public class TableImageJson
    {
        public string id;
        public string file;
        public int r0, c0, r1, c1;
        public int z;
    }

    [Serializable]
    public class DimensionJson
    {
        public int minRow, minCol, maxRow, maxCol;
    }

    [Serializable]
    public class TableTokenJson
    {
        public string id;
        public string text;
        public int r0, c0, r1, c1;
    }
}