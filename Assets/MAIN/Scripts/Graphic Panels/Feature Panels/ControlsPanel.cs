using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ControlsPanel : MonoBehaviour
{
    public static ControlsPanel Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;

    private CanvasGroupController cg = null;

    public bool isWaitingOnUserChoice { get; private set; } = false;

    private void Awake()
    {
        Instance = this;

        cg = new CanvasGroupController(this, canvasGroup);
        cg.alpha = 0;
        cg.SetInteractableState(false);
    }

    public void Show()
    {
        isWaitingOnUserChoice = true;

        cg.Show();
        cg.SetInteractableState(active: true);     
    }

    public void Hide()
    {
        isWaitingOnUserChoice = false;

        cg.Hide();
        cg.SetInteractableState(active: false);
    }
}
