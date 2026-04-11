using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using GAME;
using HISTORY;

public class CasesManager : MonoBehaviour
{
    public static CasesManager Instance { get; private set; }

    public Image caseImage;
    public TMP_Text caseName;
    public TMP_Text caseAge;
    public TMP_Text caseArea;
    public TMP_Text caseDescription;
    public TMP_Text caseAbstract;

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
    }

    public void LevelChanged()
    {
        var gm = GameManager.Instance;
        gm.IncreaseCurrentCaseLevel();
        gm.SetPercentProgress();

        casesInGame[gm.GetCurrentCaseLevel()].active = true;
        EnabledCase(gm.GetCurrentCaseLevel());
    }

    void SetCases()
    {
        casesData.Clear();
        casesData = Resources.LoadAll<CasesData>(FilePaths.resources_casesFiles).ToList();

        foreach (var caseData in casesData)
            caseData.FillFromResources(caseData.name);
    }

    private void SelectRandomCases()
    {
        casesInGame.Clear();

        casesInGame.AddRange(GetRandomCasesByLevel(CasesData.CaseLevel.facil, 3));
        casesInGame.AddRange(GetRandomCasesByLevel(CasesData.CaseLevel.intermedio, 3));
        casesInGame.AddRange(GetRandomCasesByLevel(CasesData.CaseLevel.dificil, 2));
    }

    private List<CasesInGame> GetRandomCasesByLevel(CasesData.CaseLevel level, int count)
    {
        List<CasesData> filteredCases = casesData.Where(c => c.level == level).OrderBy(c => Random.value).Take(count).ToList();
        return filteredCases.Select(c => new CasesInGame { active = false, cases = c }).ToList();
    }

    public void AssignCasesToButtons()
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

                if(i <= GameManager.Instance.GetCurrentCaseLevel())
                    casesInGame[i].active = true;

                EnabledCase(i);
            }
        }
    }

    private void EnabledCase(int level)
    {
        Button btn = casesLayoutGroup[level].GetComponent<Button>();
        Image frame = casesLayoutGroup[level].GetComponent<Image>();
        Image im = casesLayoutGroup[level].transform.Find("Image").GetComponent<Image>();
        CasesInGame game = casesInGame[level];

        btn.interactable = game.active;
        im.enabled = game.active;

        if (game.active)
        {
            im.sprite = game.cases.photo;
            frame.sprite = enbaledCase;            
        }
        else
        {
            im.sprite = null;
            frame.sprite = disabledCase;            
        }
    }

    public void OpenCasePage(int index)
    {
        CasesData data = casesInGame[index].cases;

        caseImage.sprite = data.photo;
        caseName.text = "<b>Nombre completo:</b> " + data.character.FirstCharacterToUpper();
        caseAge.text = "<b>Edad del consultante:</b> " + data.age.ToString();
        caseArea.text = "<b>Área:</b> " + data.area.ToString().FirstCharacterToUpper();
        caseDescription.text = "<b>Descripción:</b> " + data.description.FirstCharacterToUpper();
        caseAbstract.text = data.abstracts.FirstCharacterToUpper();
        foreach(Items item in HistoryManager.Instance.history.game.items)
        {
            if(item.nameFolder == caseName.text)
            {
                FolderPanel.Instance.CreateItemPrefab(item.sprite, item.nameItem);
            }
        }
    }
}