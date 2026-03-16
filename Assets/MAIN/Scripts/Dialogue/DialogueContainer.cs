using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public NameContainer nameContainer;
        public TextMeshProUGUI dialogueText;
        
        private CanvasGroupController cgController;

        public void SetDialogueColor(Color color) => dialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;
        public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;

        private bool initialized = false;

        public void Initialize()
        {
            if(initialized)
                return;
            
            cgController = new CanvasGroupController(DialogueSystem.Instance, root.GetComponent<CanvasGroup>());
            Hide();
        }

        public bool isVisible => cgController.isVisible;

        public Coroutine Show() => cgController.Show();
        public Coroutine Hide() => cgController.Hide();
    }
}
