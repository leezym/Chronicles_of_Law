using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;

namespace DIALOGUE.LogicalLines
{
    public class LL_Announce : ILogicalLine
    {
        public string keyword => "anuncio";

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            string title = line.dialogueData.rawData;
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