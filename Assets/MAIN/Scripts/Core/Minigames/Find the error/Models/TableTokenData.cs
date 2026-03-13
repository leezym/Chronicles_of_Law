using System;
using UnityEngine;

namespace EXERCISE.Model
{
    [Serializable]
    public class TableTokenData : TokenData
    {
        public string sheet;

        // Rango en grilla (inclusive). Para no-merge r0=r1 y c0=c1.
        public int r0, c0, r1, c1;
    }
}