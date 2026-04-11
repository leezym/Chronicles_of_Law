using System.Collections.Generic;
using UnityEngine;

namespace GAME
{
    [CreateAssetMenu(menuName = "Game/Minigame Definition")]
    public class MinigamesData: ScriptableObject
    {
        public GameObject game;
        public enum MinigameLevel {facil, intermedio, dificil}
        public MinigameLevel level;
        public enum MinigameType {encontrar_el_error, identificacion_derechos_vulnerados, reorganizacion_de_documentos, seleccion_de_casos}
        public MinigameType type;

        public MinigamesData(){}

        public MinigamesData(GameObject game, MinigameLevel level, MinigameType type)
        {
            this.game = game;
            this.level = level;
            this.type = type;
        }

        public static List<MinigamesData> Capture()
        {
            var mm = MinigamesManager.Instance;
            List<MinigamesData> list = mm.minigamesData;
            
            return list;
        }

        public static void Apply(List<MinigamesData> data)
        {
            var mm = MinigamesManager.Instance;
            mm.minigamesData = data;
        }

        public static List<MinigamesData> CaptureInGame()
        {
            var mm = MinigamesManager.Instance;
            List<MinigamesData> list = mm.minigamesInGame;
            
            return list;
        }

        public static void ApplyInGame(List<MinigamesData> data)
        {
            var mm = MinigamesManager.Instance;
            mm.minigamesInGame = data;
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

            name = folderParts[2];
            game = prefab;
        }
    }
}