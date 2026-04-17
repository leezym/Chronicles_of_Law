/// <summary>
/// Tablas de puntuación centralizadas para todos los minijuegos con cronómetro.
///
/// Tipo 1 – Find the Error (Documento): aciertos sobre 6, franjas 0-50s / 51-140s / 141s+
/// Tipo 2 – Find the Error (Tabla):     aciertos sobre 4, franjas 0-100s / 101-240s / 241s+
/// Tipo 3 – Drag and Drop:              intentos 1/2/3/4+, franjas 0-45s / 46-120s / 121s+
/// Tipo 4 – Case Selection MultiToggle: aciertos sobre 3, franjas 0-70s / 71-160s / 161s+
///          (el tiempo corre a partir de la primera escucha)
/// </summary>
public static class MinigamesScoring
{
    // ── Tipo 1 ── Find the Error (Documento) ────────────────────────────────
    static readonly int[,] Type1Table =
    {
        //  0-50s   51-140s  141s+
        {    10,      10,     10  },  // 0/6
        {    50,      35,     20  },  // 1/6
        {   100,      85,     70  },  // 2/6
        {   150,     135,    120  },  // 3/6
        {   200,     185,    170  },  // 4/6
        {   250,     235,    220  },  // 5/6
        {   300,     285,    270  },  // 6/6
    };

    static int Type1Band(float seconds)
    {
        if (seconds <=  50f) return 0;
        if (seconds <= 140f) return 1;
        return 2;
    }

    public static int Type1(int correctAnswers, float elapsedSeconds)
    {
        int row = Clamp(correctAnswers, 0, 6);
        int col = Type1Band(elapsedSeconds);
        return Type1Table[row, col];
    }

    // ── Tipo 2 ── Find the Error (Tabla) ────────────────────────────────────
    static readonly int[,] Type2Table =
    {
        //  0-100s  101-240s  241s+
        {    10,      10,      10  },  // 0/4
        {    75,      50,      25  },  // 1/4
        {   150,     125,     100  },  // 2/4
        {   225,     200,     175  },  // 3/4
        {   300,     275,     250  },  // 4/4
    };

    static int Type2Band(float seconds)
    {
        if (seconds <= 100f) return 0;
        if (seconds <= 240f) return 1;
        return 2;
    }

    public static int Type2(int correctAnswers, float elapsedSeconds)
    {
        int row = Clamp(correctAnswers, 0, 4);
        int col = Type2Band(elapsedSeconds);
        return Type2Table[row, col];
    }

    // ── Tipo 3 ── Drag and Drop ──────────────────────────────────────────────
    static readonly int[,] Type3Table =
    {
        //  0-45s  46-120s  121s+
        {   250,    220,    190  },  // 1 intento
        {   160,    130,    100  },  // 2 intentos
        {    70,     40,     20  },  // 3 intentos
        {    10,     10,     10  },  // 4 o más
    };

    static int Type3Band(float seconds)
    {
        if (seconds <=  45f) return 0;
        if (seconds <= 120f) return 1;
        return 2;
    }

    public static int Type3(int attempts, float elapsedSeconds)
    {
        int row = Clamp(attempts - 1, 0, 3);   // 1→0, 2→1, 3→2, 4+→3
        int col = Type3Band(elapsedSeconds);
        return Type3Table[row, col];
    }

    // ── Tipo 4 ── Case Selection MultiToggle ─────────────────────────────────
    static readonly int[,] Type4Table =
    {
        //  0-70s  71-160s  161s+
        {    10,     10,     10  },  // 0/3
        {    90,     55,     20  },  // 1/3
        {   195,    160,    125  },  // 2/3
        {   300,    265,    230  },  // 3/3
    };

    static int Type4Band(float seconds)
    {
        if (seconds <=  70f) return 0;
        if (seconds <= 160f) return 1;
        return 2;
    }

    public static int Type4(int correctAnswers, float elapsedSeconds)
    {
        int row = Clamp(correctAnswers, 0, 3);
        int col = Type4Band(elapsedSeconds);
        return Type4Table[row, col];
    }

    static int Clamp(int value, int min, int max)
        => value < min ? min : value > max ? max : value;
}