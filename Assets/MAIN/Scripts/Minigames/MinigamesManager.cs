using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GAME;
using UnityEngine.UI;
using DIALOGUE;

public class MinigamesManager : MonoBehaviour
{
    public static MinigamesManager Instance { get; private set; }

    public GameObject layoutGroup;
    public Button closeButton;
    [SerializeField] public List<MinigamesData> minigamesData = new List<MinigamesData>();
    [SerializeField] public List<MinigamesData> minigamesInGame = new List<MinigamesData>();

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeMinigames()
    {
        closeButton.GetComponent<CanvasGroup>().alpha = 0;
        closeButton.GetComponent<CanvasGroup>().blocksRaycasts = false;

        SetMinigames();
        SelectRandomMinigames();
        closeButton.onClick.AddListener(() =>
        {
            ClearLayoutGroup();
            LevelChanged();
            DialogueSystem.Instance.conversationManager.ResumeConversation();

            closeButton.interactable = false;
            closeButton.GetComponent<CanvasGroup>().alpha = 0;
            closeButton.GetComponent<CanvasGroup>().blocksRaycasts = false;
        });
    }

    public void StartGame(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, layoutGroup.transform);
        obj.transform.SetAsFirstSibling();
        obj.name = prefab.name;

        closeButton.GetComponent<CanvasGroup>().alpha = 1;
        closeButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void LevelChanged()
    {
        GameManager gm = GameManager.Instance;
        gm.SetCurrentGameLevel(gm.GetCurrentGameLevel());
    }

    void SetMinigames()
    {
        minigamesData.Clear();
        GameObject[] minigames = Resources.LoadAll<GameObject>(FilePaths.resources_minigamesFiles);

        foreach (var minigame in minigames)
        {
            MinigamesData newMinigame = new MinigamesData();

            newMinigame.FillFromResources(minigame);
            minigamesData.Add(newMinigame);
        }
    }

    private void SelectRandomMinigames()
    {
        minigamesInGame.AddRange(GetRandomMinigamesByLevel(MinigamesData.MinigameLevel.facil, 3));
        minigamesInGame.AddRange(GetRandomMinigamesByLevel(MinigamesData.MinigameLevel.intermedio, 2));
        minigamesInGame.AddRange(GetRandomMinigamesByLevel(MinigamesData.MinigameLevel.dificil, 2));
    }

    private List<MinigamesData> GetRandomMinigamesByLevel(MinigamesData.MinigameLevel level, int count)
    {
        List<MinigamesData> filteredMinigames = minigamesData.Where(m => m.level == level).OrderBy(m => Random.value).Take(count).ToList();
        return filteredMinigames;
    }

    public void ClearLayoutGroup()
    {
        GameObject gameObject = layoutGroup.transform.GetChild(0).gameObject;
        Destroy(gameObject);
    }
}
