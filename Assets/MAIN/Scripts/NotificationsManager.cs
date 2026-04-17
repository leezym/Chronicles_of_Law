using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject notificationsBackground;
    [SerializeField] private TMP_Text notificationsText;
    [SerializeField] private Button notificationsYesButton;
    [SerializeField] private Button notificationsNoButton;
    [SerializeField] private Button notificationsCloseButton;

    private Image notificationsMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        notificationsMenu = GetComponent<Image>();
    }

    private void ShowPanel(string text, bool showButtons)
    {
        notificationsMenu.raycastTarget = true;
        notificationsBackground.SetActive(true);
        notificationsText.text = text;
        notificationsYesButton.gameObject.SetActive(showButtons);
        notificationsNoButton.gameObject.SetActive(showButtons);
    }

    public void WarningNotification(string text) => ShowPanel(text, false);

    public void QuestionNotification(string text, Action function)
    {
        ShowPanel(text, true);
        SetYesButton(function);
    }

    private void SetYesButton(Action function)
    {
        notificationsYesButton.onClick.RemoveAllListeners();
        notificationsYesButton.onClick.AddListener(function.Invoke);
    }

    public void Close()
    {
        notificationsMenu.raycastTarget = false;
        notificationsBackground.SetActive(false);
        notificationsCloseButton.onClick.RemoveAllListeners();
    }
}