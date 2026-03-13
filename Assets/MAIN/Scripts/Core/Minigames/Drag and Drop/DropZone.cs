using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public RectTransform content;
    [HideInInspector]public Canvas rootCanvas;
    [Header("Marcar TRUE si es el ScrollRect Content")]
    public bool isScrollContent = false;

    void Awake()
    {
        if (content == null)
            content = transform as RectTransform;

        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var go = eventData.pointerDrag;
        if (go == null) return;

        var draggable = go.GetComponent<DraggableItem>();
        if (draggable == null) return;

        RectTransform rt = go.GetComponent<RectTransform>();

        int insertIndex = isScrollContent
            ? GetInsertIndex(eventData)
            : content.childCount;

        rt.SetParent(content, false);
        rt.SetSiblingIndex(insertIndex);
    }

    int GetInsertIndex(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            content,
            eventData.position,
            rootCanvas.worldCamera,
            out localPoint
        );

        // Recorremos hijos de arriba hacia abajo.
        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform child = content.GetChild(i) as RectTransform;
            if (child == null) continue;

            // Convertimos el "centro" del child a espacio local del content
            Vector3 childWorldCenter = child.TransformPoint(child.rect.center);
            Vector3 childLocalCenter = content.InverseTransformPoint(childWorldCenter);

            // Si el mouse está por encima del centro del child, insertamos antes de él
            if (localPoint.y > childLocalCenter.y)
                return i;
        }

        // Si no está por encima de ninguno, va al final
        return content.childCount;
    }
}