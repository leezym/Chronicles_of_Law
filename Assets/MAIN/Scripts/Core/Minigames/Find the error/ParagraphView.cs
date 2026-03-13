using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParagraphView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text tmp;

    private Action<string> _onRegionClicked;

    public TMP_Text Tmp => tmp;

    public void SetOnRegionClicked(Action<string> onRegionClicked)
    {
        _onRegionClicked = onRegionClicked;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tmp == null) return;
        if (_onRegionClicked == null) return;

        if (HighlighterToolController.Instance == null || !HighlighterToolController.Instance.IsEquipped)
            return;

        Camera cam = eventData.pressEventCamera;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp, eventData.position, cam);
        if (linkIndex == -1) return;

        TMP_LinkInfo linkInfo = tmp.textInfo.linkInfo[linkIndex];
        string regionId = linkInfo.GetLinkID();

        if (!string.IsNullOrEmpty(regionId))
            _onRegionClicked.Invoke(regionId);
    }
}