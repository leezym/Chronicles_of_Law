using UnityEngine;
using System.IO;

public class FinalMenu : MenuPage
{
    static string HOME_MUSIC = "HOME_Shadow_of_the_Verdict";
    public override void Open()
    {
        base.Open();

        AudioClip audio = Resources.Load<AudioClip>(FilePaths.resources_music + HOME_MUSIC);
        AudioManager.Instance.PlayTrack(audio, AudioBus.Music);
    }

    public override void Close()
    {
        base.Close();

        File.Delete($"{FilePaths.gameSaves}{VNGameSave.TEMP_NAME}{VNGameSave.FILE_TYPE}");
        AudioManager.Instance.StopTrack(HOME_MUSIC);
    }
}
