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
        public float points = 0f;
        public int currentLevel = 0;

        [SerializeField]
        public List<Items> items = new List<Items>();

        public static GameData Capture()
        {
            GameData data = new GameData();
            var gm = GameManager.Instance;

            data.points = gm.GetPoints();
            data.currentLevel = gm.GetCurrentLevel();
            data.items = new List<Items>(gm.items);

            return data;
        }

        public static void Apply(GameData data)
        {
            var gm = GameManager.Instance;
            var fp = FolderPanel.Instance;

            gm.SetPoints(data.points);
            gm.SetCurrentLevel(data.currentLevel);
            gm.items = new List<Items>(data.items);

            fp.ResetFolder();

            foreach(var item in gm.items)
                fp.CreateItemPrefab(item.sprite, item.nameItem);
        }
    }
}