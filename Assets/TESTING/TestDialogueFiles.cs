using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using VISUALNOVEL;
using GAME;

public class TestDialogueFiles : MonoBehaviour
{
    public static TestDialogueFiles Instance { get; private set; }

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else 
            DestroyImmediate(gameObject);
    }

    public void StartLevel()
    {
        string fullPath = AssetDatabase.GetAssetPath(CasesManager.Instance.casesInGame[GameManager.Instance.GetCurrentLevel()].cases.fileToRead);

        int resourcesIndex = fullPath.IndexOf("Resources/");
        string relativePath = fullPath.Substring(resourcesIndex + 10); // "Resources/" tiene tamaño 10

        string filePath = Path.ChangeExtension(relativePath, null);

        VNManager.Instance.LoadFile(filePath);
    }
}
