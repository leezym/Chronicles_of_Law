using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;
using DIALOGUE;

namespace History
{
    [System.Serializable]
    public class DialogueData
    { 
        public string currentDialogue = "";
        public string currentSpeaker = "";

        public string dialogueFont;
        public Color dialogueColor;
        public float dialogueScale;

        public string speakerFont;
        public Color speakerColor;
        public float speakerScale;

        public static DialogueData Capture()
        {
            DialogueData data = new DialogueData();
            var ds = DialogueSystem.Instance;
            var dialogueText = ds.dialogueContainer.dialogueText;
            var nameText = ds.dialogueContainer.nameContainer.nameText;

            data.currentDialogue = dialogueText.text;
            data.dialogueFont = FilePaths.resources_font + dialogueText.font.name;
            data.dialogueColor = dialogueText.color;
            data.dialogueScale = dialogueText.fontSize;

            data.currentSpeaker = nameText.text;
            data.speakerFont = FilePaths.resources_font + nameText.font.name;
            data.speakerColor = nameText.color;
            data.speakerScale = nameText.fontSize;
            
            return data;
        }
    }
}
