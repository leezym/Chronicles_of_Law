using System.Collections;
using System.Collections.Generic;
using History;
using UnityEngine;

public class TestHistory : MonoBehaviour
{
    public DialogueData data;
    public List<AudioData> audioData;    
    public List<GraphicData> graphicData;
    public List<CharacterData> characterData;
    void Update()
    {
        data = DialogueData.Capture();
        audioData = AudioData.Capture();
        graphicData = GraphicData.Capture();
        characterData = CharacterData.Capture();
    }
}
