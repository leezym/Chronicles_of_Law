using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GraphicPanelManager : MonoBehaviour
{
    public static GraphicPanelManager Instance { get; private set; }

    public const float DEFAULT_TRANSITION_SPEED = 3f;

    [field: SerializeField] public GraphicPanel[] allPanels { get ; private set; }

    void Awake()
    {
        Instance = this;
    }

    public GraphicPanel GetPanel(PanelType name)
    {
        foreach(var panel in allPanels)
        {
            if(panel.panelName == name)
                return panel;
        }
        return null;
    }

    public void StopVideos()
    {
        GraphicPanel panel = GetPanel(PanelType.cinematica);
        if(panel == null) return;

        foreach (var layer in panel.layers)
        {
            var graphic = layer.currentGraphic;
            if (graphic == null || !graphic.isVideo) continue;
 
            layer.Clear();
        }
    }

    public void PauseVideos()
    {
        GraphicPanel panel = GetPanel(PanelType.cinematica);
        if (panel == null) return;
 
        foreach (var layer in panel.layers)
        {
            var graphic = layer.currentGraphic;
            if (graphic == null || !graphic.isVideo) continue;
 
            graphic.video?.Pause();
            graphic.audio?.Pause();
        }
    }
 
    public void ResumeVideos()
    {
        GraphicPanel panel = GetPanel(PanelType.cinematica);
        if (panel == null) return;
 
        foreach (var layer in panel.layers)
        {
            var graphic = layer.currentGraphic;
            if (graphic == null || !graphic.isVideo || graphic.hasEnded) continue;
 
            graphic.video?.Play();
            graphic.audio?.Play();
        }
    }
}
