using System;
using System.Collections;
using System.Collections.Generic;
using HISTORY;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VISUALNOVEL;

namespace DIALOGUE
{
    public class PlayerInputManager : MonoBehaviour
    {
        static float MINIMUN_VOL = 0.0001f;
        static float MAXIMUN_VOL = 1f;
        float currentVol;
        public VNMenuManager vmm;
        public Slider sliderVolume;
        public TMP_Text muteText;
        private PlayerInput input;
        private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>(); 

        void Awake()
        {
            input = GetComponent<PlayerInput>();
            InitializeActions();
        }

        void Start()
        {
            currentVol = sliderVolume.value;
        }

        void InitializeActions()
        {
            actions.Add((input.actions["Mute"], Mute));
            actions.Add((input.actions["Next"], PromptAdvance));
            actions.Add((input.actions["Pause"], Pause));
        }

        private void OnEnable()
        {
            foreach (var inputAction in actions)
                inputAction.action.performed += inputAction.command;
        }

         private void OnDisable()
        {
            foreach (var inputAction in actions)
                inputAction.action.performed -= inputAction.command;
        }

        public void Mute()
        {
            sliderVolume.value = sliderVolume.value == MINIMUN_VOL ? currentVol : MINIMUN_VOL;
            muteText.text = sliderVolume.value == MINIMUN_VOL ? "X" : "";
        }

        private void Mute(InputAction.CallbackContext c)
        {
            Mute();            
        }

        public void SetVolume()
        {
            currentVol = sliderVolume.value > MINIMUN_VOL ? sliderVolume.value : currentVol;
            muteText.text = sliderVolume.value == MINIMUN_VOL ? "X" : "";

            AudioManager.Instance.SetVolume(sliderVolume.value);
        }

        private void PromptAdvance(InputAction.CallbackContext c)
        {
            if (!FolderPanel.Instance.isWaitingOnUserChoice)
                DialogueSystem.Instance.OnUserPrompt_Next();
        }

        private void Pause(InputAction.CallbackContext c)
        {
            vmm.TogglePauseMenu();
        }
    }
}

