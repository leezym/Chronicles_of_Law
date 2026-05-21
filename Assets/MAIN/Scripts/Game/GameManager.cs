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

        public static float CANT_LEVELS = 4f;
        public TMP_Text professionalPointsText;
        public TMP_Text socialPointsText;
        public Slider percentProgress;
        public Slider levelProgress;
        int professionalPoints { get; set; } = 0;
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
            professionalPointsText.text = "0";
            socialPointsText.text = "0";
            professionalPoints = 0;
            socialPoints = 0;
            currentCaseLevel = 0;
            currentMinigameLevel = 0;
            currentLevel = 1;

            VariableStore.CreateVariable("socialPoints", socialPoints);
            VariableStore.CreateVariable("professionalPoints", professionalPoints);
            SetLevelProgress();
            SetPercentProgress();
        }

        public int GetProfessionalPoints(){ return professionalPoints; }

        public void SetProfessionalPoints(int professionalPoints)
        {
            this.professionalPoints += professionalPoints;
            professionalPointsText.text = this.professionalPoints.ToString();
            VariableStore.TrySetValue("professionalPoints", this.professionalPoints);
        }

        public int GetSocialPoints(){ return socialPoints; }

        public void SetSocialPoints(int socialPoints)
        {
            this.socialPoints += socialPoints;
            socialPointsText.text = this.socialPoints.ToString();
            VariableStore.TrySetValue("socialPoints", this.socialPoints);
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
            Debug.Log(currentCaseLevel + 1 + "/"+(float)CasesManager.Instance.casesInGame.Count);
            Debug.Log(currentMinigameLevel + 1 + "/"+(float)MinigamesManager.Instance.minigamesInGame.Count);
            float caseLevelPercent = (currentCaseLevel + 1) / (float)CasesManager.Instance.casesInGame.Count;
            float minigameLevelPercent = (currentMinigameLevel + 1) / (float)MinigamesManager.Instance.minigamesInGame.Count;
            percentProgress.value = (caseLevelPercent + minigameLevelPercent) / 2f;
        }

        public void SetLevelProgress()
        {
            Debug.Log(currentLevel / CANT_LEVELS);
            float percent = currentLevel / CANT_LEVELS;
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