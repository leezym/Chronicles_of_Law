using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGraphicLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
        //StartCoroutine(RunningLayers());
    }

    // Update is called once per frame
    IEnumerator Running()
    {
        GraphicPanel panel = GraphicPanelManager.Instance.GetPanel(PanelType.fondo);
        GraphicLayer layer = panel.GetLayer(0, true);

        layer.SetTexture("Graphics/BG Images/background_3");

        yield return new WaitForSeconds(2);
        
        layer.SetVideo("Graphics/BG Videos/Fantasy Landscape", transitionSpeed: 0.1f, useAudio: false);
        
    }

    IEnumerator RunningLayers()
    {
        GraphicPanel panel = GraphicPanelManager.Instance.GetPanel(PanelType.fondo);
        GraphicLayer layer0 = panel.GetLayer(0, true);
        GraphicLayer layer1 = panel.GetLayer(1, true);

        layer0.SetTexture("Graphics/BG Images/background_3");        
        layer1.SetVideo("Graphics/BG Videos/Fantasy Landscape", transitionSpeed: 0.1f, useAudio: false);

        yield return null;
    }
}
