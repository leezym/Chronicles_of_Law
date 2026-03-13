using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using VISUALNOVEL;
using GAME;
using UnityEditor.VersionControl;

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
        string fullPath = AssetDatabase.GetAssetPath(CasesManager.Instance.casesInGame[GameManager.Instance.GetCurrentCaseLevel()].cases.fileToRead);

        int resourcesIndex = fullPath.IndexOf("Resources/");
        string relativePath = fullPath.Substring(resourcesIndex + 10); // "Resources/" tiene tamaño 10

        string filePath = Path.ChangeExtension(relativePath, null);

        VNManager.Instance.LoadFile(filePath);
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
