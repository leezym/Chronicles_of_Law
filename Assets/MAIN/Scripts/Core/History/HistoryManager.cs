using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using Unity.VisualScripting;
using UnityEngine;

namespace HISTORY
{
    public class HistoryManager : MonoBehaviour
    {
        public static HistoryManager Instance { get; private set; }
        public HistoryState history = new HistoryState();

        void Awake()
        {
            Instance = this;  
        }

        void Start()
        {
            DialogueSystem.Instance.onClear += LogCurrentState;
        }

        public void LogCurrentState()
        {
            history = HistoryState.Capture();
        }
    }
}