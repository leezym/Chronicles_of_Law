using UnityEngine;
using UnityEngine.UI;
using GAME;
using TMPro;
using Unity.VisualScripting;

public class CasesButton : MonoBehaviour
{
    int indexCaseInGame;

    public void Setup(int index)
    {        
        indexCaseInGame = index;
        CasesData data = CasesManager.Instance.casesInGame[index].cases;

        this.gameObject.name = data.level.ToString().FirstCharacterToUpper() + 
            " - " + data.area.ToString().FirstCharacterToUpper() + 
            " - " + data.name.FirstCharacterToUpper();
    }

    public void OnClick()
    {
        GameManager.Instance.OpenCasePage(indexCaseInGame);
    }
}