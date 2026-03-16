using System.Collections.Generic;
using UnityEngine;

namespace GAME
{  
    [System.Serializable]
    public class CasesInGame
    {   
        public bool active;
        public CasesData cases;

        public CasesInGame(){}

        public static List<CasesInGame> Capture()
        {
            List<CasesInGame> list = new List<CasesInGame>();
            var cm = CasesManager.Instance;
            list = cm.casesInGame;
            
            return list;
        }

        public static void Apply(List<CasesInGame> data)
        {
            var cm = CasesManager.Instance;
            cm.casesInGame = data;
        }
    }
}