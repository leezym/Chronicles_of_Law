using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Case Definition")]
public class CasesData : ScriptableObject
{
    public enum CaseLevel {facil, intermedio, dificil}
    public CaseLevel level;
    public TextAsset fileToRead;
    public Sprite photo;
    public string character;
    public int age;
    public enum CaseArea {civil, familia, laboral, publico}
    public CaseArea area;
    public string description;
    public string abstracts;

    public CasesData(){}

    public CasesData(CaseLevel level, TextAsset fileToRead, Sprite photo, string character, int age, CaseArea area, string description, string abstracts)
    {
        this.level = level;
        this.fileToRead = fileToRead;
        this.photo = photo;
        this.character = character;
        this.age = age;
        this.area = area;
        this.description = description;
        this.abstracts = abstracts;
    }

    public static List<CasesData> Capture()
    {
        List<CasesData> list = new List<CasesData>();
        var cm = CasesManager.Instance;
        list = cm.casesData;
        
        return list;
    }

    public static void Apply(List<CasesData> data)
    {
        var cm = CasesManager.Instance;
        cm.casesData = data;
    }

    public void FillFromResources(string assetName)
    {
        string[] folderParts = assetName.Split('.');
        string levelString = folderParts[0].ToLower();
        string areaString = folderParts[1].ToLower();

        if (System.Enum.TryParse(levelString, out CaseLevel parsedLevel))
        {
            level = parsedLevel;
        }
        else
        {
            Debug.LogWarning("Nivel desconocido: " + levelString);
        }

        if (System.Enum.TryParse(areaString, out CaseArea parsedArea))
        {
            area = parsedArea;
        }
        else
        {
            Debug.LogWarning("Área desconocida: " + areaString);
        }

        Object[] assets = Resources.LoadAll(FilePaths.resources_casesFiles + assetName);

        foreach (Object obj in assets)
        {
            if (obj is TextAsset text)
            {
                if (text.name.Contains(assetName))
                    fileToRead = text;

                if (text.name.ToLower().Contains("abstract"))
                    abstracts = text.text;

                if (text.name.ToLower().Contains("info"))
                {
                    string[] infoLines = text.text.Split('\n');
                    if (infoLines.Length >= 3)
                    {
                        character = infoLines[0].Trim();
                        if (int.TryParse(infoLines[1].Trim(), out int parsedAge))
                        {
                            age = parsedAge;
                        }
                        description = infoLines[2].Trim();
                    }
                    else
                    {
                        Debug.LogWarning($"El archivo {text.name} no tiene el formato esperado.");
                    }
                }
            }

            if (obj is Sprite sprite)
            {
                if (sprite.name.ToLower().Contains("photo"))
                    photo = sprite;
            }
        }
    }
}