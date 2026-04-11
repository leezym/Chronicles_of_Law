using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VISUALNOVEL;
namespace GAME
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public TMP_Text profressionalPointsText;
        public TMP_Text socialPointsText;
        public Slider percentProgress;
        public Slider levelProgress;
        int profressionalPoints { get; set; } = 0;
        int socialPoints { get; set; } = 0;
        int currentCaseLevel { get; set; } = 0;
        int currentMinigameLevel { get; set; } = 0;
        int currentLevel { get; set; } = 1;

        [SerializeField] public List<Items> items = new List<Items>();
        
        public enum Gender {F, M}
        [field: SerializeField] public Gender currentGameGender { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void InitializeGame()
        {
            profressionalPointsText.text = "0";
            socialPointsText.text = "0";
            profressionalPoints = 0;
            socialPoints = 0;
            currentCaseLevel = 0;
            currentMinigameLevel = 0;
            currentLevel = 1;

            VariableStore.CreateVariable("socialPoints", socialPoints);
            SetLevelProgress();
            SetPercentProgress();
        }

        public int GetProfessionalPoints(){ return profressionalPoints; }

        public void SetProfessionalPoints(int profressionalPoints)
        {
            this.profressionalPoints += profressionalPoints;
            profressionalPointsText.text = this.profressionalPoints.ToString();
        }

        public int GetSocialPoints(){ return socialPoints; }

        public void SetSocialPoints(int socialPoints)
        {
            this.socialPoints += socialPoints;
            socialPointsText.text = this.socialPoints.ToString();
        }

        public int GetCurrentCaseLevel(){ return currentCaseLevel; }

        public void SetCurrentCaseLevel(int currentCaseLevel)
        {
            this.currentCaseLevel = currentCaseLevel;
        }

        public void IncreaseCurrentCaseLevel()
        {
            currentCaseLevel ++;
        }

        public int GetCurrentMinigameLevel(){ return currentMinigameLevel; }

        public void SetCurrentMinigameLevel(int currentMinigameLevel)
        {
            this.currentMinigameLevel = currentMinigameLevel;
        }

        public void IncreaseCurrentMinigameLevel()
        {
            currentMinigameLevel ++;
        }

        public int GetCurrentLevel(){ return currentLevel; }

        public void SetCurrentLevel(int currentLevel)
        {
            this.currentLevel = currentLevel;
        }

        public void IncreaseCurrentLevel()
        {
            currentLevel ++;
        }

        public void SetPercentProgress()
        {
            float caseLevelPercent = (currentCaseLevel + 1) / (float)CasesManager.Instance.casesInGame.Count;
            float minigameLevelPercent = (currentMinigameLevel + 1) / (float)MinigamesManager.Instance.minigamesInGame.Count;
            percentProgress.value = (caseLevelPercent + minigameLevelPercent) / 2f;
        }

        public void SetLevelProgress()
        {
            float percent = currentLevel / 4f;
            levelProgress.value = percent;
        }

        public Gender GetCurrentGender(){ return currentGameGender; }

        public void SetGenderF() => currentGameGender = Gender.F;
        public void SetGenderM() => currentGameGender = Gender.M;

        public void LevelChanged()
        {            
            IncreaseCurrentLevel();
            SetLevelProgress();
            VNManager.Instance.StartLevel();
        }
    }
}