using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAudioHook : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler,
    IPointerDownHandler,
    IBeginDragHandler,
    IEndDragHandler
{

    [Header("Audio Event IDs")]
    public string hoverId = "";          // ej: "ui.hover"
    public string clickId = "";          // ej: "ui.click.button"
    public string downId = "";           // ej: "ui.down"
    public string beginDragLoopId = "";  // ej: "sfx.drag.loop" (AudioEvent loop=true)
    public string endDragId = "";        // ej: "sfx.drag.drop" (one-shot)

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(hoverId))
            AudioManager.Instance.PlayEvent(hoverId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(clickId))
            AudioManager.Instance.PlayEvent(clickId);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(downId))
            AudioManager.Instance.PlayEvent(downId);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(beginDragLoopId))
            AudioManager.Instance.PlayEvent(beginDragLoopId); // tu AudioManager evita duplicados por id
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(beginDragLoopId))
            AudioManager.Instance.StopLoop(beginDragLoopId);

        if (!string.IsNullOrWhiteSpace(endDragId))
            AudioManager.Instance.PlayEvent(endDragId);
    }
}