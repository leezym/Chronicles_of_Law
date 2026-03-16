using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsPanel : MonoBehaviour
{
    public static ItemsPanel Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image itemImage;
    [SerializeField] public Button closeButton;

    private CanvasGroupController cg = null;

    public bool isWaitingOnUserChoice { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);
        cg.alpha = 0;
        cg.SetInteractableState(active: false);
    }

    public void Show(Sprite item)
    {
        isWaitingOnUserChoice = true;

        cg.Show();
        cg.SetInteractableState(active: true);

        itemImage.sprite = item;

        RectTransform parentRect = itemImage.transform.parent.GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;

        float aspectRatio = item.rect.height / item.rect.width;
        float newHeight = parentWidth * aspectRatio;

        RectTransform imageRect = itemImage.GetComponent<RectTransform>();
        imageRect.sizeDelta = new Vector2(parentWidth, newHeight);
    }

    public void Hide()
    {
        isWaitingOnUserChoice = false;

        cg.Hide();
        cg.SetInteractableState(active: false);
    }    
}
