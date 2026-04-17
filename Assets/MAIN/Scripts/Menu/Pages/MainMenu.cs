using System.IO;
using CHARACTERS;
using UnityEngine;
using VISUALNOVEL;

public class MainMenu : MenuPage
{
    static string HOME_MUSIC = "HOME_Shadow_of_the_Verdict";
    public override void Open()
    {
        base.Open();

        VNManager.Instance.continueButton.interactable = File.Exists($"{FilePaths.gameSaves}{VNGameSave.TEMP_NAME}{VNGameSave.FILE_TYPE}");

        AnnouncePanel.Instance.Hide();
        ChoicePanel.Instance.Hide();
        FolderPanel.Instance.Hide();
        ItemsPanel.Instance.Hide();
        ControlsPanel.Instance.Hide();

        AudioManager.Instance.Stop();
        GraphicPanelManager.Instance.StopVideos();

        foreach(var ch in CharacterManager.Instance.GetCharacterConfigArray())
        {
            Character character = CharacterManager.Instance.GetCharacter(ch.name, createIfDoesNotExist: false);
            if(character != null)
                character.Destroy();
        }

        AudioClip audio = Resources.Load<AudioClip>(FilePaths.resources_music + HOME_MUSIC);
        AudioManager.Instance.PlayTrack(audio, AudioBus.Music);
    }

    public override void Close()
    {
        base.Close();

        AudioManager.Instance.StopTrack(HOME_MUSIC);
    }
}
