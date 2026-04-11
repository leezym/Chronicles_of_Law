using System.Collections.Generic;
using CHARACTERS;
using GAME;

public class MainMenu : MenuPage
{
    public override void Open()
    {
        base.Open();

        AnnouncePanel.Instance.Hide();
        ChoicePanel.Instance.Hide();
        FolderPanel.Instance.Hide();
        ItemsPanel.Instance.Hide();

        AudioManager.Instance.Stop();
        GraphicPanelManager.Instance.StopVideos();

        foreach(var ch in CharacterManager.Instance.GetCharacterConfigArray())
        {
            Character character = CharacterManager.Instance.GetCharacter(ch.name, createIfDoesNotExist: false);
            if(character != null)
                character.Destroy();
        }
    }
}
