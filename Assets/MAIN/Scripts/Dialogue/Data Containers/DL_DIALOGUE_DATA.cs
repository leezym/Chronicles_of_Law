using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DIALOGUE
{
    public class DL_DIALOGUE_DATA
    {
        CultureInfo culture = CultureInfo.CreateSpecificCulture("es-ES");

        public string rawData { get; private set; } = string.Empty;
        public List<DIALOGUE_SEGMENT> segments;
        private const string segmentIdentifierPatter = @"\{[cCbB]\}|\{[eE][cCbB]\s\d*\,?\d*\}";
        
        public DL_DIALOGUE_DATA(string rawDialogue)
        {
            this.rawData = rawDialogue;
            segments = RipSegments(rawDialogue);
        }

        public List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
        {
            List<DIALOGUE_SEGMENT> segments = new List<DIALOGUE_SEGMENT>();
            MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPatter);
            
            int lastIndex = 0;
            //FIND THE FIRST OR ONLY SEGMENT IN THE FILE
            DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
            segment.dialogue = (matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index));
            segment.startSignal = DIALOGUE_SEGMENT.StartSignal.NONE;
            segment.signalDelay = 0;
            segments.Add(segment);

            if(matches.Count == 0)
                return segments;
            else
                lastIndex = matches[0].Index;

            for(int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                segment = new DIALOGUE_SEGMENT();

                //Get the start signal fot the segment
                string signalMatch = match.Value; //{A}
                signalMatch = signalMatch.Substring(1, match.Length - 2);
                string[] signalSplit = signalMatch.Split(' ');

                segment.startSignal = (DIALOGUE_SEGMENT.StartSignal) Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal), signalSplit[0].ToUpper());

                //Get the signal delay
                if(signalSplit.Length > 1)
                    float.TryParse(signalSplit[1], NumberStyles.Float, culture, out segment.signalDelay);

                //Get the dialogue for the segment
                int nextIndex = i + 1 < matches.Count ? matches[i+1].Index : rawDialogue.Length;
                segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
                lastIndex = nextIndex;

                segments.Add(segment);
            }

            return segments;
        }

        public struct DIALOGUE_SEGMENT
        {
            public string dialogue;
            public StartSignal startSignal;
            public float signalDelay;

            public enum StartSignal{NONE, B, C, EC, EB} // NINGUNO, BORRAR, CONTINUAR, ESPERAR Y CONTINUAR, ESPERAR Y BORRAR

            public bool appendText => (startSignal == StartSignal.C || startSignal == StartSignal.EC);
        }
    }
}