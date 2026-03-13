using System;

namespace EXERCISE.Model
{
    [Serializable]
    public class TokenData
    {
        public string id;
        public string text;

        // “Habilitado para ser subrayado”
        public bool enabled;

        // Si da puntos
        public bool isCorrect;
        public float points;

        // Runtime (no persistente)
        [NonSerialized] public bool selected;
        [NonSerialized] public bool locked;
    }
}