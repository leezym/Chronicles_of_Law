using System.Collections;

namespace DIALOGUE.LogicalLines
{
    public class LL_Announce : ILogicalLine
    {
        public string keyword => "anuncio";

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            string title = line.dialogueData.rawData;

            GraphicLayer layer = GraphicPanelManager.Instance.GetPanel(PanelType.cinematica).GetLayer(0);
            if (layer != null)
                yield return layer.WaitForCurrentVideoIfAny();
                
            AnnouncePanel panel = AnnouncePanel.Instance;
            panel.Show(title);

            while(panel.isWaitingOnUserChoice)
                yield return null;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }
}