using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace DIALOGUE
{
    public class DL_SPEAKER_DATA
    {
        public string rawData { get; private set; } = string.Empty;
        public string name;
        public Vector2 castPosition;
        public List<(int layer, string expression)> CastExpressions { get; set; }

        public bool isCastingPosition = false;
        public bool isCastingExpressions => CastExpressions.Count > 0;

        public bool makeCharacterEnter = false;

        private const string POSITIONCAST_ID = " en ";
        private const string EXPRESSIONCAST_ID = " [";
        private const char AXISDELIMITER_ID = ':';
        private const char EXPRESSIONLAYER_JOINER = ',';
        private const char EXPRESSIONLAYER_DELIMITER = ':';

        private const string ENTER_KEYWORD = "entra ";

        private string ProcessKeywords(string rawSpeaker)
        {
            if(rawSpeaker.StartsWith(ENTER_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                rawSpeaker = rawSpeaker.Substring(ENTER_KEYWORD.Length);
                makeCharacterEnter = true;
            }

            return rawSpeaker;
        }

        public DL_SPEAKER_DATA(string rawSpeaker)
        {
            rawData = rawSpeaker;
            rawSpeaker = ProcessKeywords(rawSpeaker);

            string pattern = @$"{POSITIONCAST_ID}|{POSITIONCAST_ID.ToUpper()}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

            //Populate this data to avoid null references to values
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();

            //If there are no matches, then this entire line is the speaker name
            if(matches.Count == 0)
            {
                name = rawSpeaker;
                return;
            }

            //Otherwise, isolate the speakername from the casting data
            int index = matches[0].Index;
            name = rawSpeaker.Substring(0, index);

            for(int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0, endIndex = 0;

                if(match.Value == POSITIONCAST_ID)
                {
                    isCastingPosition = true;

                    startIndex = match.Index + POSITIONCAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPos.Split(AXISDELIMITER_ID, System.StringSplitOptions.RemoveEmptyEntries);

                    float.TryParse(axis[0], out castPosition.x);
                    
                    if(axis.Length > 1)
                        float.TryParse(axis[1], out castPosition.y);
                }
                else if(match.Value == EXPRESSIONCAST_ID)
                {
                    startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    CastExpressions = castExp.Split(EXPRESSIONLAYER_JOINER)
                    .Select(x => {
                        var parts = x.Trim().Split(EXPRESSIONLAYER_DELIMITER);

                        if(parts.Length == 2) //Give two instructions (Body and Face)
                            return (int.Parse(parts[0]), parts[1]);
                        else //Give one instruction (Face)
                            return (2, parts[0]); //Use inexistent layer (2) to be replace after in HandleSpeakerLogic
                    }).ToList();
                }
            }
        }
    }
}