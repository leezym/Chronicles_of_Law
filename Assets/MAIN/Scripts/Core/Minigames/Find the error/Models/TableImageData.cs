using System;

namespace EXERCISE.Model
{
    [Serializable]
    public class TableImageData
    {
        public string id;
        public string file;   // relativo a la carpeta del ejercicio
        public int r0, c0, r1, c1;
        public int z;
    }
}