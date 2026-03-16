using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GAME;
using DIALOGUE;

public class OrderValidator : MonoBehaviour
{
    [Header("References")]
    public RectTransform scrollContent;
    public RectTransform poolParent;
    public Button validateButton;
    Button closeButton => MinigamesManager.Instance.closeButton;
    GameManager gameManager => GameManager.Instance;

    [Header("Correct order (WordToken.wordId)")]
    public List<string> correctOrder = new List<string>();

    [Header("Scoring")]
    public int pointsPerCorrectMatch = 10;

    [Header("Rules")]
    public bool requireAllInsideScroll = true;
    public bool lockDragAfterValidate = true;

    [Header("Feedback colors")]
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;

    bool validated;
    int expectedTotalItems;

    float nextCheckTime;
    const float CHECK_INTERVAL = 0.10f;

    void Start()
    {
        expectedTotalItems = correctOrder.Count;

        UpdateValidateButtonState(force: true);
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
        {
            var child = parent.GetChild(i);
            if (child.GetComponent<WordToken>() != null)
                count++;
        }
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
            int poolWords = CountWordTokens(poolParent);

            if (poolParent != null)
                canValidate = (poolWords == 0) && (scrollWords == correctOrder.Count);
            else
                canValidate = (scrollWords == correctOrder.Count);
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
            int poolWords = CountWordTokens(poolParent);

            bool ok = (poolParent != null)
                ? (poolWords == 0 && scrollWords == expectedTotalItems)
                : (scrollWords == expectedTotalItems);

            if (!ok)
            {
                Debug.Log("Aún faltan palabras por colocar dentro del correo.");
                return;
            }
        }

        validated = true;
        if (validateButton != null) validateButton.interactable = false;

        int matches = 0;

        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Transform child = scrollContent.GetChild(i);
            var token = child.GetComponent<WordToken>();

            // Si no es palabra real, lo ignoramos para scoring/feedback
            if (token == null) continue;

            bool isCorrect = (i < correctOrder.Count &&
                              !string.IsNullOrWhiteSpace(token.wordId) &&
                              token.wordId == correctOrder[i]);

            ApplyBorderFeedback(child, isCorrect);
            if (isCorrect) matches++;
        }

        int totalPoints = matches * pointsPerCorrectMatch;
        gameManager.SetPoints(totalPoints);

        if (lockDragAfterValidate)
        {
            DisableAllDrag();            
        }

        closeButton.interactable = true;
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
        if (outline != null)
        {
            outline.effectColor = c;
            return;
        }
    }
}