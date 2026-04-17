using UnityEngine;
using TMPro;

public class MinigamesTimer : MonoBehaviour
{
    [Header("UI (opcional)")]
    [Tooltip("Texto donde se muestra MM:SS mientras el cronómetro corre.")]
    public TMP_Text timerText;

    public float ElapsedSeconds { get; private set; } = 0f;
    public bool IsRunning       { get; private set; } = false;

    void Update()
    {
        if (!IsRunning) return;

        ElapsedSeconds += Time.deltaTime;
        RefreshText();
    }

    /// <summary>Arranca el cronómetro. Si ya corre, no hace nada.</summary>
    public void StartTimer()
    {
        if (IsRunning) return;
        IsRunning = true;
    }

    /// <summary>Detiene el cronómetro sin reiniciarlo.</summary>
    public void StopTimer()
    {
        IsRunning = false;
        RefreshText();
    }

    /// <summary>Detiene y reinicia el cronómetro a cero.</summary>
    public void ResetTimer()
    {
        IsRunning      = false;
        ElapsedSeconds = 0f;
        RefreshText();
    }

    void RefreshText()
    {
        if (timerText == null) return;
        int m = (int)(ElapsedSeconds / 60f);
        int s = (int)(ElapsedSeconds % 60f);
        timerText.text = $"{m:00}:{s:00}";
    }
}