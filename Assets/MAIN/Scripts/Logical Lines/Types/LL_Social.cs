using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GAME;
using UnityEngine;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;

namespace DIALOGUE.LogicalLines
{
    public class LL_Social : ILogicalLine
    {
        public string keyword => "social";
        private const string pointRegexPattern = @"\[([-+]?\d*\.?\d+)\|([-+]?\d*\.?\d+)\]";

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            var gm = GameManager.Instance;
            var currentConversation = DialogueSystem.Instance.conversationManager.conversation;
            var progress = DialogueSystem.Instance.conversationManager.conversationProgress;
            EncapsulatedData data = RipEncapsulationData(currentConversation, progress, ripHeaderAndEncapsulators: true, parentStartingIndex: currentConversation.fileStartIndex);
            List<Social> socials = GetChoicesFromData(data);

            string title = line.dialogueData.rawData;
            string[] socialTitles = socials.Select(c => c.title).ToArray();

            GraphicLayer layer = GraphicPanelManager.Instance.GetPanel(PanelType.cinematica).GetLayer(0);
            if (layer != null)
                yield return layer.WaitForCurrentVideoIfAny();
            
            ChoicePanel panel = ChoicePanel.Instance;
            panel.Show(title, socialTitles);

            while(panel.isWaitingOnUserChoice)
                yield return null;
            
            Social selectedChoice = socials[panel.lastDecision.answerIndex];
            gm.SetSocialPoints(selectedChoice.socialPoints);
            gm.SetProfessionalPoints(selectedChoice.questionsPoints);

            Conversation newConversation = new Conversation(selectedChoice.resultLines, file: currentConversation.file, fileStartIndex: selectedChoice.startIndex, fileEndIndex: selectedChoice.endIndex);
            DialogueSystem.Instance.conversationManager.conversation.SetProgress(data.endingIndex - currentConversation.fileStartIndex);
            DialogueSystem.Instance.conversationManager.EnqueuePriority(newConversation);
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }

        private List<Social> GetChoicesFromData(EncapsulatedData data)
        {
            List<Social> socials = new List<Social>();
            int encapsulationDepth = 0;
            bool isFirstChoice = true;

            Social social = new Social
            {
                title = string.Empty,
                resultLines = new List<string>(),
                socialPoints = 0,
                questionsPoints = 0
            };

            int socialIndex = 0, i = 0;
            for(i = 1; i < data.lines.Count; i++)
            {
                var line = data.lines[i];
                if(IsChoiceStart(line) && encapsulationDepth == 1)
                {
                    if(!isFirstChoice)
                    {
                        social.startIndex = data.startingIndex + (socialIndex + 1);
                        social.endIndex = data.startingIndex + (i - 1);
                        socials.Add(social);
                        social = new Social
                        {
                            title = string.Empty,
                            resultLines = new List<string>(),
                            socialPoints = 0,
                            questionsPoints = 0
                        };
                    }     

                    socialIndex = i;
                    social.title = line.Trim().Substring(1);

                    Regex pointRegex = new Regex(pointRegexPattern);
                    MatchCollection matches = pointRegex.Matches(line);

                    foreach (Match match in matches)
                    {
                        string numberStr1 = match.Groups[1].Value;
                        string numberStr2 = match.Groups[2].Value;
                        int number1, number2;

                        if (int.TryParse(numberStr1, out number1) && int.TryParse(numberStr2, out number2))
                        {
                            social.socialPoints = number1;
                            social.questionsPoints = number2;

                            social.title = line.Remove(match.Index, match.Length).Trim().Substring(1);
                        }
                        else
                        {
                            Debug.LogError("No se pudieron convertir los números.");
                        }
                    }
                    
                    isFirstChoice = false;

                    continue;
                }

                AddLineToResults(line, ref social, ref encapsulationDepth);                
            }

            if(!socials.Contains(social))
            {
                social.startIndex = data.startingIndex + (socialIndex + 1);
                social.endIndex = data.startingIndex + (i - 2);
                socials.Add(social);
            }

            return socials;
        }

        private void AddLineToResults(string line, ref Social social, ref int encapsulationDepth)
        {
            line.Trim();

            if(IsEncapsulationStart(line))
            {
                if(encapsulationDepth > 0)
                    social.resultLines.Add(line);

                encapsulationDepth++;
                return;
            }

            if(IsEncapsulationEnd(line))
            {
                encapsulationDepth--;

                if(encapsulationDepth > 0)
                    social.resultLines.Add(line);
                
                return;
            }

            social.resultLines.Add(line);
        }
        
        private struct Social
        {
            public string title;
            public List<string> resultLines;
            public int startIndex;
            public int endIndex;
            public int socialPoints;
            public int questionsPoints;
        }
    }
}