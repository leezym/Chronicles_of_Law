using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using GAME;
using System.IO;

public class CasesManager : MonoBehaviour
{
    public static CasesManager Instance { get; private set; }

    [SerializeField] public List<CasesData> casesData = new List<CasesData>();
    [SerializeField] public List<CasesInGame> casesInGame = new List<CasesInGame>();
    [SerializeField] private List<GameObject> casesLayoutGroup;

    public Sprite disabledCase;
    public Sprite enbaledCase;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeCases()
    {
        SetCases();
        SelectRandomCases();
        AssignCasesToButtons();
        TestDialogueFiles.Instance.StartLevel();
    }

    public void ContinueCases()
    {
        TestGameSave tgm = new TestGameSave();
        tgm.Load();
        
        AssignCasesToButtons();
    }

    public void LevelChanged()
    {
        var gm = GameManager.Instance;
        int level = gm.GetCurrentLevel() + 1;

        casesInGame[level].active = true;
        EnabledCase(casesInGame[level], casesLayoutGroup[level]);

        gm.SetCurrentLevel(level);
    }

    void SetCases()
    {
        string basePath = "Items";
        string fullPath = Application.dataPath + "/MAIN/Resources/" + basePath;
        string[] directories = Directory.GetDirectories(fullPath);

        foreach (string directory in directories)
        {
            string folderName = Path.GetFileName(directory);
            string[] folderParts = folderName.Split('.');
            string levelString = folderParts[0].ToLower();
            string areaString = folderParts[1].ToLower();

            CasesData newCase = new CasesData();

            if (System.Enum.TryParse(levelString, out CasesData.CaseLevel parsedLevel))
            {
                newCase.level = parsedLevel;
            }
            else
            {
                Debug.LogWarning("Nivel desconocido: " + levelString);
                continue;
            }

            if (System.Enum.TryParse(areaString, out CasesData.CaseArea parsedArea))
            {
                newCase.area = parsedArea;
            }
            else
            {
                Debug.LogWarning("Área desconocida: " + areaString);
                continue;
            }

            Object[] folderContents = Resources.LoadAll(basePath + "/" + folderName);
            foreach (Object obj in folderContents)
            {
                if (obj is TextAsset textAsset)
                {
                    if (textAsset.name == "abstract")
                    {
                        newCase.abstracts = textAsset.text;
                    }
                    else if (textAsset.name == folderName)
                    {
                        newCase.fileToRead = textAsset;
                    }
                    else if (textAsset.name == "info")
                    {
                        string[] infoLines = textAsset.text.Split('\n');
                        if (infoLines.Length >= 3)
                        {
                            newCase.name = infoLines[0].Trim();
                            if (int.TryParse(infoLines[1].Trim(), out int parsedAge))
                            {
                                newCase.age = parsedAge;
                            }
                            newCase.description = infoLines[2].Trim();
                        }
                        else
                        {
                            Debug.LogWarning("El archivo 'info' no tiene el formato esperado: " + textAsset.name);
                        }
                    }
                }
                else if (obj is Texture2D texture && obj.name == "photo")
                {
                    newCase.photo = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                }
            }

            casesData.Add(newCase);
        }
    }

    private void SelectRandomCases()
    {
        casesInGame.AddRange(GetRandomCasesByLevel(CasesData.CaseLevel.facil, 3));
        casesInGame.AddRange(GetRandomCasesByLevel(CasesData.CaseLevel.intermedio, 3));
        casesInGame.AddRange(GetRandomCasesByLevel(CasesData.CaseLevel.dificil, 3));
        
    }

    private List<CasesInGame> GetRandomCasesByLevel(CasesData.CaseLevel level, int count)
    {
        List<CasesData> filteredCases = casesData.Where(c => c.level == level).OrderBy(c => Random.value).Take(count).ToList();
        return filteredCases.Select(c => new CasesInGame { active = false, cases = c }).ToList();
    }

    private void AssignCasesToButtons()
    {
        for (int i = 0; i < casesLayoutGroup.Count; i++)
        {
            if (i < casesInGame.Count)
            {
                CasesButton buttonScript = casesLayoutGroup[i].GetComponent<CasesButton>();
                buttonScript.Setup(i);

                Button btn = casesLayoutGroup[i].GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(buttonScript.OnClick);

                if(i <= GameManager.Instance.GetCurrentLevel())
                    casesInGame[i].active = true;

                EnabledCase(casesInGame[i], casesLayoutGroup[i]);
            }
        }
    }

    private void EnabledCase(CasesInGame casesInGame, GameObject casesLayoutGroup)
    {
        if(casesInGame.active)
        {
            casesLayoutGroup.GetComponent<Image>().sprite = enbaledCase;
            casesLayoutGroup.GetComponent<Button>().interactable = true;
        }
        else
        {
            casesLayoutGroup.GetComponent<Image>().sprite = disabledCase;
            casesLayoutGroup.GetComponent<Button>().interactable = false;
        }
    }
}