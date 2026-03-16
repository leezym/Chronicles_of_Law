using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GAME;

public class ToggleAnswerValidator : MonoBehaviour
{
    [Header("Referencias")]
    public Toggle answerToggle;
    public Button validateButton;
    public TMP_Text feedbackText;
    Button closeButton => MinigamesManager.Instance.closeButton;
    public GameManager gameManager => GameManager.Instance;

    [Header("Configuración")]
    public bool correctAnswerIsOn = true;
    public float pointsForCorrectAnswer;
    public float pointsForIncorrectAnswer;

    private bool alreadyAnswered = false;

    void Start()
    {
        if (feedbackText != null)
            feedbackText.text = "";

        if (validateButton != null)
            validateButton.onClick.AddListener(ValidateAnswer);
    }

    void ValidateAnswer()
    {
        if (alreadyAnswered) return;

        bool userAnswer = answerToggle.isOn;

        if (userAnswer == correctAnswerIsOn)
        {
            feedbackText.text = "Correcto";
            feedbackText.color = Color.green;            
            gameManager.SetPoints(pointsForCorrectAnswer);
        }
        else
        {
            feedbackText.text = "Incorrecto";
            feedbackText.color = Color.red;
            gameManager.SetPoints(pointsForIncorrectAnswer);
        }

        answerToggle.interactable = false;
        validateButton.interactable = false;
        closeButton.interactable = true;
        alreadyAnswered = true;
    }
}