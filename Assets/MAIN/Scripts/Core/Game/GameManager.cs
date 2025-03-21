using System.Collections;
using System.Collections.Generic;
using HISTORY;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GAME
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public TMP_Text pointsText;
        float points { get; set; } = 0f;
        int currentLevel { get; set; } = 0;

        public Image caseImage;
        public TMP_Text caseName;
        public TMP_Text caseAge;
        public TMP_Text caseArea;
        public TMP_Text caseDescription;
        public TMP_Text caseAbstract;
        [SerializeField] public List<Items> items = new List<Items>();

        private void Awake()
        {
            Instance = this;
        }

        public float GetPoints(){ return points; }

        public void SetPoints(float points)
        {
            this.points = points;
            pointsText.text = points.ToString();
        }

        public int GetCurrentLevel(){ return currentLevel; }

        public void SetCurrentLevel(int currentLevel)
        {
            this.currentLevel = currentLevel;
        }

        public void OpenCasePage(int index)
        {
            CasesData data = CasesManager.Instance.casesInGame[index].cases;

            caseImage.sprite = data.photo;
            caseName.text = data.name.FirstCharacterToUpper();
            caseAge.text = data.age.ToString();
            caseArea.text = data.area.ToString().FirstCharacterToUpper();
            caseDescription.text = data.description.FirstCharacterToUpper();
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
}