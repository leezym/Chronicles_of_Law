using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GAME;
using Unity.VisualScripting;

public class FolderPanel : MonoBehaviour
{
    public static FolderPanel Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;

    public GameObject page_1;
    public GameObject page_2;
    public GameObject page_3_abstract;
    public GameObject page_3_documents;
    public GameObject page_4;

    public Image caseDocumentImage;
    public Button caseDocumentImageZoomButton;

    [SerializeField] private GameObject itemButtonPrefab;
    public GridLayoutGroup documentsLayoutGroup;

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

    public void Show()
    {
        page_1.SetActive(true);
        page_2.SetActive(false);
        page_3_abstract.SetActive(false);
        page_3_documents.SetActive(false);
        page_4.SetActive(false);
        
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

    public void CreateItemPrefab(Sprite item, string name)
    {        
        GameObject newButtonObject = Instantiate(itemButtonPrefab, documentsLayoutGroup.transform);
        newButtonObject.SetActive(true);

        TextMeshProUGUI text = newButtonObject.GetComponentInChildren<TextMeshProUGUI>();
        text.text = name;

        Button newButton = newButtonObject.GetComponent<Button>();

        newButton.onClick.RemoveAllListeners();
        newButton.onClick.AddListener(() => {
            caseDocumentImage.sprite = item;
            
            RectTransform parentRect = caseDocumentImage.transform.parent.GetComponent<RectTransform>();
            float parentWidth = parentRect.rect.width;

            float aspectRatio = item.rect.height / item.rect.width;
            float newHeight = parentWidth * aspectRatio;

            RectTransform imageRect = caseDocumentImage.GetComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(parentWidth, newHeight);
        });

        ItemsPanel.Instance.closeButton.onClick.AddListener(() => isWaitingOnUserChoice = false );
    }

    public void AddItemPrefab(Sprite item, string nameItem,string nameFolder)
    {
        GameManager.Instance.items.Add(new Items(nameFolder, nameItem, item));
    }

    public void ZoomItem(Image item)
    {
        caseDocumentImageZoomButton.onClick.RemoveAllListeners();
        caseDocumentImageZoomButton.onClick.AddListener(() => {
            ItemsPanel.Instance.Show(item.sprite);
            isWaitingOnUserChoice = true;
        });
    }

    public void ResetFolder()
    {
        Transform parent = documentsLayoutGroup.transform;
        foreach (Transform child in parent)
        {
            if (child.name == "Button(Clone)")
            {
                Destroy(child.gameObject);
            }
        }
    }     
}