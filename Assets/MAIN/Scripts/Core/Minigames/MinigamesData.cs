using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

namespace GAME
{
    [System.Serializable]
    public class MinigamesData
    {
        public string name;
        public GameObject game;
        public enum MinigameLevel {facil, intermedio, dificil}
        public MinigameLevel level;
        public enum MinigameType {encontrar_el_error, excel_liquidacion, identificacion_derechos_vulnerados, reorganizacion_de_documentos, seleccion_de_casos}
        public MinigameType type;

        public MinigamesData(){}

        public MinigamesData(string name, GameObject game, MinigameLevel level, MinigameType type)
        {
            this.name = name;
            this.game = game;
            this.level = level;
            this.type = type;
        }

        public static List<MinigamesData> Capture()
        {
            List<MinigamesData> list = new List<MinigamesData>();
            var mm = MinigamesManager.Instance;
            list = mm.minigamesData;
            
            return list;
        }

        public static void Apply(List<MinigamesData> data)
        {
            var mm = MinigamesManager.Instance;
            mm.minigamesData = data;
        }

        public void FillFromResources(GameObject prefab)
        {
            string[] folderParts = prefab.name.Split('.');
            string levelString = folderParts[0].ToLower();
            string typeString = folderParts[1].ToLower();

            if (System.Enum.TryParse(levelString, out MinigameLevel parsedLevel))
            {
                level = parsedLevel;
            }
            else
            {
                Debug.LogWarning("Nivel desconocido: " + levelString);
            }

            if (System.Enum.TryParse(typeString, out MinigameType parsedArea))
            {
                type = parsedArea;
            }
            else
            {
                Debug.LogWarning("Tipo desconocido: " + typeString);
            }

            name = prefab.transform.GetChild(0).name;
            game = prefab;
            /*game.alpha = 1;
            game.interactable = true;
            game.blocksRaycasts = true;*/
        }
    }
}