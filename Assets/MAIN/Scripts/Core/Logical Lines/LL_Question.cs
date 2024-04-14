using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;

namespace DIALOGUE.LogicalLines
{
    public class LL_Question : ILogicalLine
    {
        public string keyword => "pregunta";        
        private const string pointRegexPattern = @"\[([-+]?\d*\,?\d+)\]";

        private float totlaPoints = 0f;

        public IEnumerator Execute(DIALOGUE_LINE line)
        {
            var currentConversation = DialogueSystem.Instance.conversationManager.conversation;
            var progress = DialogueSystem.Instance.conversationManager.conversationProgress;
            EncapsulatedData data = RipEncapsulationData(currentConversation, progress);
            List<Question> questions = GetChoicesFromData(data);
            
            string title = line.dialogueData.rawData;
            ChoicePanel panel = ChoicePanel.Instance;
            string[] choiceTitles = questions.Select(c => c.title).ToArray();

            panel.Show(title, choiceTitles);

            while(panel.isWaitingOnUserChoice)
                yield return null;
            
            Question selectedChoice = questions[panel.lastDecision.answerIndex];
            totlaPoints += selectedChoice.points;
            Debug.Log(totlaPoints);

            //pdte guardar puntuaciones (totalPoints)

            DialogueSystem.Instance.conversationManager.conversation.SetProgress(data.endingIndex);
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }

        private List<Question> GetChoicesFromData(EncapsulatedData data)
        {
            List<Question> questions = new List<Question>();
            int encapsulationDepth = 0;
            bool isFirstChoice = true;

            Question question = new Question
            {
                title = string.Empty,
                points = 0f
            };

            foreach (var line in data.lines.Skip(1))
            {
                if(IsChoiceStart(line) && encapsulationDepth == 1)
                {
                    if(!isFirstChoice)
                    {
                        questions.Add(question);
                        question = new Question
                        {
                            title = string.Empty,
                            points = 0f
                        };
                    }                   

                    Regex pointRegex = new Regex(pointRegexPattern);
                    MatchCollection matches = pointRegex.Matches(line);

                    foreach (Match match in matches)
                    {
                        string numberStr = match.Groups[1].Value;
                        float number;

                        if (float.TryParse(numberStr, out number))
                        {
                            question.points = number;
                            question.title = line.Remove(match.Index, match.Length).Trim().Substring(1);
                        }
                        else
                        {
                            Debug.LogError("No se pudo convertir el número a decimal.");
                        }
                    }

                    isFirstChoice = false;

                    continue;
                }

                AddLineToResults(line, ref encapsulationDepth);
            }

            if(!questions.Contains(question))
                questions.Add(question);

            return questions;
        }

        private void AddLineToResults(string line, ref int encapsulationDepth)
        {
            line.Trim();

            if(IsEncapsulationStart(line))
            {
                encapsulationDepth++;
                return;
            }

            if(IsEncapsulationEnd(line))
            {
                encapsulationDepth--;                
                return;
            }
        }

        private struct Question
        {
            public string title;
            public float points;
        }
    }
}