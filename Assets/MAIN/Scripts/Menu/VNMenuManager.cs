using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using VISUALNOVEL;

public class VNMenuManager : MonoBehaviour
{
    private MenuPage activePage = null;
    private bool isOpen = false;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;

    private CanvasGroupController rootCG;

    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
        OpenMainMenu();
    }

    private MenuPage GetPage(MenuPage.MenuType menuType)
    {
        return pages.FirstOrDefault(page => page.menuType == menuType);
    }

    public void OpenMainMenu()
    {
        var page = GetPage(MenuPage.MenuType.Main);
        OpenPage(page);
    }

    public void OpenCharacterMenu()
    {
        var page = GetPage(MenuPage.MenuType.CharacterSelector);
        OpenPage(page);
    }

    public void OpenFinalMenu()
    {
        var page = GetPage(MenuPage.MenuType.Final);
        OpenPage(page);
    }

    public void TogglePauseMenu()
    {
        if (MinigamesManager.Instance.IsMinigameActive)
        {
            NotificationsManager.Instance.WarningNotification("No puedes pausar el juego mientras estás en un Minijuego.");
            return;
        }

        VNManager.Instance.saveButton.interactable = !CasesManager.Instance.IsCaseActive;
 
        var page = GetPage(MenuPage.MenuType.Pause);
        var navPage = GetPage(MenuPage.MenuType.Navigation);
 
        if (activePage == navPage)
            OpenPage(page);
        else if (activePage == page)
            OpenPage(navPage);
    }

    public void OpenNavigationMenu()
    {
        var page = GetPage(MenuPage.MenuType.Navigation);
        OpenPage(page);
    }

    private void OpenPage(MenuPage page)
    {
        if(page == null)
            return;
        
        if(activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if(!isOpen)
            OpenRoot();
    }
    
    public void OpenRoot()
    {
        rootCG.Show();
        rootCG.SetInteractableState(true);
        isOpen = true;
    }

    public void CloseRoot()
    {
        rootCG.Hide();
        rootCG.SetInteractableState(false);
        isOpen = false;
    }
}
