using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GAME;
using DIALOGUE;

public class OrderValidator : MonoBehaviour
{
    [Header("References")]
    public RectTransform scrollContent;
    public RectTransform poolParent;
    public Button validateButton;
    public TMP_Text attemptsText;       // Opcional: muestra "Intentos: N"
    Button closeButton  => MinigamesManager.Instance.closeButton;
    GameManager gameManager => GameManager.Instance;

    [Header("Correct order (WordToken.wordId)")]
    public List<string> correctOrder = new List<string>();

    [Header("Rules")]
    public bool requireAllInsideScroll = true;
    public bool lockDragAfterValidate  = true;

    [Header("Feedback colors")]
    public Color correctColor   = Color.green;
    public Color incorrectColor = Color.red;

    MinigamesTimer timer;

    bool validated;
    int  attempts = 0;
    int  expectedTotalItems;

    float nextCheckTime;
    const float CHECK_INTERVAL = 0.10f;

    void Start()
    {
        expectedTotalItems = correctOrder.Count;

        // Crear y arrancar el cronómetro
        timer = gameObject.AddComponent<MinigamesTimer>();
        timer.StartTimer();

        UpdateValidateButtonState(force: true);
        RefreshAttemptsText();
    }

    void Update()
    {
        if (validated) return;
        if (Time.unscaledTime < nextCheckTime) return;

        nextCheckTime = Time.unscaledTime + CHECK_INTERVAL;
        UpdateValidateButtonState(force: false);
    }

    int CountWordTokens(RectTransform parent)
    {
        if (parent == null) return 0;
        int count = 0;
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).GetComponent<WordToken>() != null)
                count++;
        return count;
    }

    void UpdateValidateButtonState(bool force)
    {
        if (validateButton == null || scrollContent == null) return;

        if (validated)
        {
            if (validateButton.interactable) validateButton.interactable = false;
            return;
        }

        bool canValidate;
        if (!requireAllInsideScroll)
        {
            canValidate = true;
        }
        else
        {
            int scrollWords = CountWordTokens(scrollContent);
            int poolWords   = CountWordTokens(poolParent);
            canValidate = (poolParent != null)
                ? (poolWords == 0 && scrollWords == correctOrder.Count)
                : (scrollWords == correctOrder.Count);
        }

        if (force || validateButton.interactable != canValidate)
            validateButton.interactable = canValidate;
    }

    public void ValidateAndScore()
    {
        if (validated) return;
        if (scrollContent == null || gameManager == null) return;

        if (requireAllInsideScroll)
        {
            int scrollWords = CountWordTokens(scrollContent);
            int poolWords   = CountWordTokens(poolParent);
            bool ok = (poolParent != null)
                ? (poolWords == 0 && scrollWords == expectedTotalItems)
                : (scrollWords == expectedTotalItems);

            if (!ok)
            {
                Debug.Log("Aún faltan palabras por colocar dentro del correo.");
                return;
            }
        }

        // Contar intento
        attempts++;
        RefreshAttemptsText();

        // Evaluar aciertos posición a posición
        int matches    = 0;
        int tokenIndex = 0;

        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Transform child = scrollContent.GetChild(i);
            var token       = child.GetComponent<WordToken>();
            if (token == null) continue;

            bool isCorrect = (tokenIndex < correctOrder.Count &&
                              !string.IsNullOrWhiteSpace(token.wordId) &&
                              token.wordId == correctOrder[tokenIndex]);

            ApplyBorderFeedback(child, isCorrect);
            if (isCorrect) matches++;
            tokenIndex++;
        }

        bool allCorrect = matches == expectedTotalItems;

        if (allCorrect)
        {
            // ── Victoria ──────────────────────────────────────────────────
            validated = true;
            timer.StopTimer();

            int points = MinigamesScoring.Type3(attempts, timer.ElapsedSeconds);
            gameManager.SetProfessionalPoints(points);

            if (lockDragAfterValidate) DisableAllDrag();
            if (validateButton != null) validateButton.interactable = false;

            var cg = closeButton.GetComponent<CanvasGroup>();
            if (cg != null) { cg.alpha = 1; cg.interactable = true; cg.blocksRaycasts = true; }
            closeButton.interactable = true;
        }
        // Si no es correcto: el jugador puede seguir intentando.
        // UpdateValidateButtonState en Update() reactiva REVISAR automáticamente.
    }

    void RefreshAttemptsText()
    {
        if (attemptsText != null)
            attemptsText.text = $"Intentos: {attempts}";
    }

    void DisableAllDrag()
    {
        DisableDragInParent(scrollContent);
        if (poolParent != null) DisableDragInParent(poolParent);
    }

    void DisableDragInParent(RectTransform parent)
    {
        if (parent == null) return;
        for (int i = 0; i < parent.childCount; i++)
        {
            var d = parent.GetChild(i).GetComponent<DraggableItem>();
            if (d != null) d.enabled = false;
        }
    }

    void ApplyBorderFeedback(Transform item, bool isCorrect)
    {
        if (item == null) return;
        Color c = isCorrect ? correctColor : incorrectColor;
        var outline = item.GetComponent<Outline>();
        if (outline != null) outline.effectColor = c;
    }
}