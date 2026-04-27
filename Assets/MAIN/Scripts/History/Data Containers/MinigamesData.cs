using System.Collections.Generic;
using UnityEngine;

namespace GAME
{
    [System.Serializable]                                                                                                                                                                
    public class MinigamesSaveEntry                                                                                                                                                      
    {                                                                                                                                                                                    
        public string prefabName;                                                                                                                                                        
    }

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

        public static List<MinigamesSaveEntry> Capture()
        {
            var mm = MinigamesManager.Instance;
            var entries = new List<MinigamesSaveEntry>();                                                                                                                                
            foreach (var md in mm.minigamesData)                                                                                                                                         
                if (md.game != null)                                                                                                                                                     
                    entries.Add(new MinigamesSaveEntry { prefabName = md.game.name });                                                                                                   
            
            return entries;
        }

        public static void Apply(List<MinigamesSaveEntry> data)
        {
            if (data == null) return;

            var mm = MinigamesManager.Instance;
            mm.minigamesData = new List<MinigamesData>();

            foreach (var entry in data)                                                                                                                                                  
            {                                                                                                                                                                                          GameObject prefab = Resources.Load<GameObject>($"{FilePaths.resources_minigamesFiles}{entry.prefabName}");                                                               
                if (prefab != null)                                                                                                                                                      
                {                                                                                                                                                                        
                    MinigamesData md = new MinigamesData();                                                                                                                              
                    md.FillFromResources(prefab);                                                                                                                                        
                mm.minigamesData.Add(md);                                                                                                                                            
                }                                                                                                                                                                        
                else                                                                                                                                                                     
                Debug.LogWarning($"Minigame prefab not found: {entry.prefabName}");                                                                                                  
            } 
        }

        public static List<MinigamesSaveEntry> CaptureInGame()  
        {
            var mm = MinigamesManager.Instance;
            var entries = new List<MinigamesSaveEntry>();

            foreach (var md in mm.minigamesInGame)                                                                                                                                       
                if (md.game != null)                                                                                                                                                     
                  entries.Add(new MinigamesSaveEntry { prefabName = md.game.name });
                                                                                                        
            return entries;
        }

        public static void ApplyInGame(List<MinigamesSaveEntry> data)
        {
            var mm = MinigamesManager.Instance;
            mm.minigamesInGame = new List<MinigamesData>();

            foreach (var entry in data)                                                                                                                                                  
            {                                                                                                                                                                            
                GameObject prefab = Resources.Load<GameObject>($"{FilePaths.resources_minigamesFiles}{entry.prefabName}");                                                               
                if (prefab != null)                                                                                                                                                      
                {                                                                                                                                                                        
                    MinigamesData md = new MinigamesData();                                                                                                                              
                    md.FillFromResources(prefab);                                                                                                                                        
                    mm.minigamesInGame.Add(md);                                                                                                                                          
                }                                                                                                                                                                        
                else                                                                                                                                                                     
                    Debug.LogWarning($"Minigame prefab not found: {entry.prefabName}");                                                                                                  
            }
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
