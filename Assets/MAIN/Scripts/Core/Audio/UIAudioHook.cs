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
        if (!string.IsNullOrWhiteSpace(id))
            AudioManager.Instance.PlayEvent(id);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(id))
            AudioManager.Instance.PlayEvent(id);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(id))
            AudioManager.Instance.PlayEvent(id);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(id))
            AudioManager.Instance.PlayEvent(id);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!string.IsNullOrWhiteSpace(loopId))
            AudioManager.Instance.StopLoop(loopId);

        if (!string.IsNullOrWhiteSpace(endId))
            AudioManager.Instance.PlayEvent(endId);
    }
}