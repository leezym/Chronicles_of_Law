using DIALOGUE;
using UnityEngine;

public class PauseMenu : MenuPage
{
    public override void Open()
    {
        base.Open();
        
        GraphicPanelManager.Instance.PauseVideos();
        DialogueSystem.Instance.conversationManager.PauseConversation();
        AudioManager.Instance.Pause();
    }

    public override void Close()
    {
        base.Close();

        GraphicPanelManager.Instance.ResumeVideos();
        DialogueSystem.Instance.conversationManager.ResumeConversation();
        AudioManager.Instance.Resume();
    }
}
