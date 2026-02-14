using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GraphicLayer
{
    public const string LAYER_OBJECT_NAME_FORMAT = "Layer: {0}";
    public int layerDepth = 0;
    public Transform panel;
    public PanelType ownerPanelId;
    private bool ShouldClearCurrentGraphicOnVideoFinish => ownerPanelId == PanelType.cinematica;
    public System.Action OnCurrentVideoFinished;

    public GraphicObject currentGraphic = null;
    public List<GraphicObject> oldGraphics = new List<GraphicObject>();

    public Coroutine SetTexture(string filePath, float transitionSpeed = 1f)
    {
        Texture tex = Resources.Load<Texture>(filePath);

        if(tex == null)
        {
            Debug.LogError($"Could not load graphic texture from path '{filePath}.' Please ensure it exists within Resources!");
            return null;
        }

        return SetTexture(tex, transitionSpeed, filePath);
    }

    public Coroutine SetTexture(Texture tex, float transitionSpeed = 1f, string filePath = "")
    {
        return CreateGraphic(tex, transitionSpeed, filePath);
    }

    public Coroutine SetVideo(string filePath, float transitionSpeed = 1f, bool useAudio = true, bool loop = false)
    {
        VideoClip clip = Resources.Load<VideoClip>(filePath);

        if(clip == null)
        {
            Debug.LogError($"Could not load graphic video from path '{filePath}.' Please ensure it exists within Resources!");
            return null;
        }

        return SetVideo(clip, transitionSpeed, useAudio, loop, filePath);
    }

    public Coroutine SetVideo(VideoClip clip, float transitionSpeed = 1f, bool useAudio = true, bool loop = false, string filePath = "", AudioBus bus = AudioBus.Ambience)
    {
        return CreateGraphic(clip, transitionSpeed, filePath, useAudio, loop, bus);
    }

    private Coroutine CreateGraphic<T>(T graphicData, float transitionSpeed, string filePath, bool useAudioForVideo = true, bool loop = false, AudioBus bus = AudioBus.Ambience)
    {
        GraphicObject newGraphic = null;

        if(graphicData is Texture)
            newGraphic = new GraphicObject(this, filePath, graphicData as Texture);
        else if(graphicData is VideoClip)
        {
            newGraphic = new GraphicObject(this, filePath, graphicData as VideoClip, useAudioForVideo, loop, bus);
            newGraphic.Finished += () =>
            {
                // Siempre visar que el video terminó (aunque ya no sea el currentGraphic)
                OnCurrentVideoFinished?.Invoke();

                // Solo limpiar si ese video sigue siendo el actual
                if (currentGraphic != newGraphic)
                    return;

                if (ShouldClearCurrentGraphicOnVideoFinish)
                {
                    GraphicPanel panel = GraphicPanelManager.Instance.GetPanel(ownerPanelId);

                    if (layerDepth == -1)
                        panel.Clear(transitionSpeed);
                    else
                    {
                        GraphicLayer graphicLayer = panel.GetLayer(layerDepth);
                        if (graphicLayer == null)
                        {
                            Debug.LogError($"Could not clear layer [{layerDepth}] on panel '{panel.panelName}'");
                            return;
                        }
                        graphicLayer.Clear(transitionSpeed);
                    }
                }
            };
        }
        
        if(currentGraphic != null && !oldGraphics.Contains(currentGraphic))
            oldGraphics.Add(currentGraphic);

        currentGraphic = newGraphic;

        return currentGraphic.FadeIn(transitionSpeed);
    }

    public void DestroyOldGraphics()
    {
        foreach(var g in oldGraphics)
            Object.Destroy(g.renderer.gameObject);
        
        oldGraphics.Clear();
    }

    public void Clear(float transitionSpeed = 1)
    {
        if(currentGraphic != null)
            currentGraphic.FadeOut(transitionSpeed);
        
        foreach(var g in oldGraphics)
            g.FadeOut(transitionSpeed);
    }

    public IEnumerator WaitForCurrentVideoIfAny()
    {
        bool videoFinished = false;
        System.Action handler = null;

        // Caso 1: no hay gráfico
        if (currentGraphic == null)
        {
            yield break;
        }

        var g = currentGraphic;

        // Caso 2a: no es video o ya terminó
        if (!g.isVideo || g.hasEnded)
        {
            yield break;
        }

        // Caso 2b: es video, pero ya no está en reproducción (por ejemplo pausado al final)
        if (g.isVideo && g.video != null && !g.video.isPlaying && g.video.isPrepared && g.video.time >= g.video.length - 0.05f)
            yield break;

        // Caso 3: es video y sigue en reproducción
        handler = () => videoFinished = true;
        OnCurrentVideoFinished += handler;

        while (!videoFinished)
            yield return null;

        OnCurrentVideoFinished -= handler;
    }
}
