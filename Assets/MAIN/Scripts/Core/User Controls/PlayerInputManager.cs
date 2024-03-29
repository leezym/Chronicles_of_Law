using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class PlayerInputManager : MonoBehaviour
    {
        private const string CLIC_SOUND_PATH = "Audio/SFX/interface-click";
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                //Clic sound
                AudioManager.Instance.PlaySoundEffect(CLIC_SOUND_PATH);

                PromptAdvance();
            }
        }

        public void PromptAdvance()
        {
            DialogueSystem.Instance.OnUserPrompt_Next();
        }
    }
}

