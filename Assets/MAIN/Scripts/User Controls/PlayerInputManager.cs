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
        float currentVol;
        public Slider sliderVolume;
        public GameObject muteImage;
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
            muteImage.SetActive(false);
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
            muteImage.SetActive(sliderVolume.value == MINIMUN_VOL ? true : false);
        }

        private void Mute(InputAction.CallbackContext c)
        {
            Mute();            
        }

        public void SetVolume()
        {
            currentVol = sliderVolume.value > MINIMUN_VOL ? sliderVolume.value : currentVol;
            muteImage.SetActive(sliderVolume.value == MINIMUN_VOL ? true : false);

            AudioManager.Instance.SetVolume(sliderVolume.value);
        }

        private void PromptAdvance(InputAction.CallbackContext c)
        {
            if (!FolderPanel.Instance.isWaitingOnUserChoice)
                DialogueSystem.Instance.OnUserPrompt_Next();
        }

        private void Pause(InputAction.CallbackContext c)
        {
            VNManager.Instance.vmm.TogglePauseMenu();
        }
    }
}

