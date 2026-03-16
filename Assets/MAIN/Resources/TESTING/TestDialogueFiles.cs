using UnityEngine;
using UnityEditor;
using System.IO;
using VISUALNOVEL;
using GAME;

public class TestDialogueFiles : MonoBehaviour
{
    public static TestDialogueFiles Instance { get; private set; }

    public TextAsset texto_cinematica;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else 
            DestroyImmediate(gameObject);
    }

    public void StartLevel()
    {
        TextAsset file = CasesManager.Instance.casesInGame[GameManager.Instance.GetCurrentCaseLevel()].cases.fileToRead;
        VNManager.Instance.LoadFile(file);
    }

    public void StartF()
    {
        GameManager.Instance.SetGenderF();
        StartCinematic();
    }

    public void StartM()
    {
        GameManager.Instance.SetGenderM();
        StartCinematic();
    }
    public void StartCinematic()
    {        
        string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, $"Nivel.{GameManager.Instance.GetCurrentLevel()}");
        VNManager.Instance.LoadFile(filePath);
    }
}
