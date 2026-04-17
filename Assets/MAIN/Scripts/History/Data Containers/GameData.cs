using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using AYellowpaper.SerializedCollections;
using GAME;

namespace HISTORY
{
    [System.Serializable]
    public class GameData
    {
        public int professionalPoints = 0;
        public int socialPoints = 0;
        public int currentCaseLevel = 0;
        public int currentMinigameLevel = 0;
        public int currentLevel = 0;

        [SerializeField]
        public List<Items> items = new List<Items>();

        public static GameData Capture()
        {
            GameData data = new GameData();
            var gm = GameManager.Instance;

            data.professionalPoints = gm.GetProfessionalPoints();
            data.socialPoints = gm.GetSocialPoints();
            data.currentCaseLevel = gm.GetCurrentCaseLevel();
            data.currentMinigameLevel = gm.GetCurrentMinigameLevel();
            data.currentLevel = gm.GetCurrentLevel();
            data.items = new List<Items>(gm.items);

            return data;
        }

        public static void Apply(GameData data)
        {
            var gm = GameManager.Instance;
            var fp = FolderPanel.Instance;

            gm.SetProfessionalPoints(data.professionalPoints);
            gm.SetSocialPoints(data.socialPoints);
            gm.SetCurrentCaseLevel(data.currentCaseLevel);
            gm.SetCurrentMinigameLevel(data.currentMinigameLevel);
            gm.SetCurrentLevel(data.currentLevel);
            gm.items = new List<Items>(data.items);

            fp.ResetFolder();

            foreach(var item in gm.items)
                fp.CreateItemPrefab(item.sprite, item.nameItem);

            VariableStore.TrySetValue("socialPoints", data.socialPoints);
            VariableStore.TrySetValue("professionalPoints", data.professionalPoints);
        }
    }
}