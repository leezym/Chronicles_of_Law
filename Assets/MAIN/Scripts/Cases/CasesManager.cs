using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using GAME;
using VISUALNOVEL;

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
        gm.SetCurrentCaseLevel(gm.GetCurrentCaseLevel());

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
}