using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas rootCanvas;
    public RectTransform dragOverlay;

    RectTransform rt;
    CanvasGroup cg;

    [HideInInspector] public Transform originalParent;
    [HideInInspector] public int originalIndex;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
        if (dragOverlay == null && rootCanvas != null)
            dragOverlay = rootCanvas.transform as RectTransform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DropCatcher.Instance?.EnableCatcher();

        originalParent = rt.parent;
        originalIndex = rt.GetSiblingIndex();

        rt.SetParent(dragOverlay, true);
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 overlayLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragOverlay,
            eventData.position,
            rootCanvas.worldCamera,
            out overlayLocal
        );

        rt.localPosition = overlayLocal;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DropCatcher.Instance?.DisableCatcher();

        cg.blocksRaycasts = true;

        // Si nadie lo recibió, vuelve a su lugar original
        if (rt.parent == dragOverlay)
        {
            rt.SetParent(originalParent, false);
            rt.SetSiblingIndex(originalIndex);
        }
    }
}