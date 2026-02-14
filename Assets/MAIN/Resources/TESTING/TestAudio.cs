using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

namespace TESTING
{
    public class TestAudio : MonoBehaviour
    {
        void Start()
        {
            //StartCoroutine(Run());
        }

        Character CreateCharacter(String name) => CharacterManager.Instance.CreateCharacter(name);

        IEnumerator Run()
        {
            AudioManager.Instance.PlayTrack("Audio/Music/acustico", volumeCap: 0.5f, bus: AudioBus.Music);
            yield return null;
        }

        private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            AudioManager.Instance.PlayEvent("ui.click.mouse");

        if (Input.GetKeyDown(KeyCode.W))
            AudioManager.Instance.PlayEvent("sfx.test");
    }

    }

    /* 
        ------------------ Caso Drag & Drop
        private AudioHandle _dragLoop;

        public void OnBeginDrag()
        {
            _dragLoop = AudioManager.Instance.PlayLoopById("sfx.drag.loop");
        }

        public void OnEndDrag()
        {
            AudioManager.Instance.StopLoop(_dragLoop);
            AudioManager.Instance.PlayEvent("sfx.drag.drop");
        }

    */
}