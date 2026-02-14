using UnityEngine;

[CreateAssetMenu(menuName = "Audio/UI Audio Theme", fileName = "UIAudioTheme")]
public class UIAudioThemeSO : ScriptableObject
{
    [Header("Defaults (Event IDs)")]
    public string hoverId = "ui.hover";
    public string clickId = "ui.click.button";
    public string downId = "";

    [Header("Drag Defaults")]
    public string beginDragLoopId = ""; // "sfx.drag.loop"
    public string endDragId = "";       // "sfx.drag.drop"
}