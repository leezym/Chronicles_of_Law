using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GAME;

public class MultitoggleAnswerValidator : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip audioClip;
    public Button playButton;

    [Header("Toggles")]
    public List<ToggleItem> toggleItems = new List<ToggleItem>();

    [Header("UI")]
    public Button validateButton;

    public int maxSelections = 3;

    Button closeButton  => MinigamesManager.Instance.closeButton;
    GameManager gameManager => GameManager.Instance;

    MinigamesTimer timer;

    bool audioPlayedOnce = false;
    bool validated       = false;
    int  selectedCount   = 0;

    AudioSource activeVoiceSource = null;

    void Start()
    {
        timer = gameObject.AddComponent<MinigamesTimer>();

        if (validateButton != null)
            validateButton.onClick.AddListener(Validate);

        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        foreach (var item in toggleItems)
        {
            if (item.toggle == null) continue;
            ToggleItem captured = item;
            item.toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(captured, isOn));
        }
    }

    void OnPlayClicked()
    {
        if (audioClip == null)
        {
            Debug.LogWarning("AudioMultiToggleMinigame: audioClip no asignado.");
            return;
        }

        if (activeVoiceSource != null && activeVoiceSource.isPlaying)
            activeVoiceSource.Stop();

        activeVoiceSource = AudioManager.Instance.PlayVoice(audioClip);

        if (!audioPlayedOnce)
        {
            audioPlayedOnce = true;
            timer.StartTimer();
        }
    }

    void OnToggleChanged(ToggleItem item, bool isOn)
    {
        if (validated) return;

        if (isOn)
        {
            selectedCount++;
        }
        else
        {
            selectedCount = Mathf.Max(0, selectedCount - 1);
        }

        EnforceSelectionLimit();
    }

    void EnforceSelectionLimit()
    {
        bool limitReached = selectedCount >= maxSelections;

        foreach (var item in toggleItems)
        {
            if (item.toggle == null) continue;

            if (limitReached && !item.toggle.isOn)
            {
                // Desactivar sin disparar el listener
                item.toggle.interactable = false;
            }
            else if (!limitReached)
            {
                item.toggle.interactable = true;
            }
        }
    }

    void Validate()
    {
        if (validated) return;
        //if (selectedCount != maxSelections) return;

        validated = true;
        timer.StopTimer();

        activeVoiceSource.Stop();
        playButton.interactable = false;

        foreach (var item in toggleItems)
            if (item.toggle != null) item.toggle.interactable = false;

        if (validateButton != null) validateButton.interactable = false;

        int correctHits   = 0;
        int incorrectHits = 0;

        foreach (var item in toggleItems)
        {
            if (item.toggle == null) continue;

            if (item.toggle.isOn)
            {
                ApplyToggleFeedback(item, item.isCorrect);
                if (item.isCorrect) correctHits++;
                else                incorrectHits++;
            }
            else if (item.isCorrect)
            {
                ApplyToggleFeedback(item, null);
            }
        }

        int points = MinigamesScoring.Type4(correctHits, timer.ElapsedSeconds);
        gameManager.SetProfessionalPoints(points);

        var cg = closeButton.GetComponent<CanvasGroup>();
        if (cg != null) { cg.alpha = 1; cg.interactable = true; cg.blocksRaycasts = true; }
        closeButton.interactable = true;
    }

    void ApplyToggleFeedback(ToggleItem item, bool? isCorrect)
    {
        if (item.toggle == null) return;

        TMP_Text label = item.toggle.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            if (isCorrect == true)       label.color = Color.green;
            else if (isCorrect == false) label.color = Color.red;
            else                         label.color = Color.orange;
        }

        Outline outline = item.toggle.GetComponent<Outline>();
        if (outline != null)
        {
            if (isCorrect == true)       outline.effectColor = Color.green;
            else if (isCorrect == false) outline.effectColor = Color.red;
            else                         outline.effectColor = Color.orange;
        }
    }

}

[System.Serializable]
public class ToggleItem
{
    [Tooltip("Referencia al componente Toggle en la escena")]
    public Toggle toggle;

    [Tooltip("¿Es esta una de las respuestas correctas?")]
    public bool isCorrect = false;
}