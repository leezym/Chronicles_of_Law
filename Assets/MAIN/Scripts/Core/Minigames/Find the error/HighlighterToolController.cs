using UnityEngine;
using UnityEngine.UI;

public class HighlighterToolController : MonoBehaviour
{
    public static HighlighterToolController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private RectTransform toolRect;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private RectTransform documentContent;

    [Header("Optional raycast control")]
    [SerializeField] private Graphic[] graphicsToToggleRaycast;

    [Header("Settings")]
    [SerializeField] private Vector2 dragOffset = new Vector2(20f, -20f);
    [SerializeField] private bool returnOnOutsideClick = true;

    public bool IsEquipped { get; private set; }

    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalAnchoredPosition;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (toolRect == null)
            toolRect = GetComponent<RectTransform>();

        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>();

        originalParent = toolRect.parent;
        originalSiblingIndex = toolRect.GetSiblingIndex();
        originalAnchoredPosition = toolRect.anchoredPosition;
        originalAnchorMin = toolRect.anchorMin;
        originalAnchorMax = toolRect.anchorMax;
        originalPivot = toolRect.pivot;

        SetGraphicsRaycast(true);
    }

    private void Update()
    {
        if (!IsEquipped)
            return;

        FollowMouse();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToHome();
            return;
        }

        if (returnOnOutsideClick && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerInsideDocument())
            {
                ReturnToHome();
            }
        }
    }

    public void ToggleEquipped()
    {
        if (IsEquipped)
            ReturnToHome();
        else
            Equip();
    }

    public void Equip()
    {
        if (IsEquipped) return;
        if (toolRect == null || rootCanvas == null) return;

        IsEquipped = true;

        // Re-parent al canvas para que quede encima de todo
        toolRect.SetParent(rootCanvas.transform, true);
        toolRect.SetAsLastSibling();

        // Anchors neutros para moverlo libremente por canvas
        toolRect.anchorMin = new Vector2(0.5f, 0.5f);
        toolRect.anchorMax = new Vector2(0.5f, 0.5f);
        toolRect.pivot = new Vector2(0.5f, 0.5f);

        // Mientras sigue al mouse, NO debe bloquear clicks del documento
        SetGraphicsRaycast(false);

        FollowMouse();
    }

    public void ReturnToHome()
    {
        if (toolRect == null) return;

        IsEquipped = false;

        toolRect.SetParent(originalParent, true);
        toolRect.SetSiblingIndex(originalSiblingIndex);

        // Restaurar layout original
        toolRect.anchorMin = originalAnchorMin;
        toolRect.anchorMax = originalAnchorMax;
        toolRect.pivot = originalPivot;
        toolRect.anchoredPosition = originalAnchoredPosition;

        // Vuelve a ser clickeable en su panel
        SetGraphicsRaycast(true);
    }

    private void FollowMouse()
    {
        if (rootCanvas == null || toolRect == null) return;

        RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out Vector2 localPoint
        );

        toolRect.anchoredPosition = localPoint + dragOffset;
    }

    private bool IsPointerInsideDocument()
    {
        if (documentContent == null || rootCanvas == null)
            return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            documentContent,
            Input.mousePosition,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera
        );
    }

    private void SetGraphicsRaycast(bool enabled)
    {
        if (graphicsToToggleRaycast == null || graphicsToToggleRaycast.Length == 0)
            return;

        foreach (var g in graphicsToToggleRaycast)
        {
            if (g != null)
                g.raycastTarget = enabled;
        }
    }
}