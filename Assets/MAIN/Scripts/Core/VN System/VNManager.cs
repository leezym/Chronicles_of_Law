using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace VISUALNOVEL
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        public void LoadFile(string filePath)
        {
            List<string> lines = new List<string>();
            TextAsset file = Resources.Load<TextAsset>(filePath);
            
            try
            {
                lines = FileManager.ReadTextAsset(file);
            }
            catch
            {
                Debug.LogError($"Dialogue file at path 'Resources/{filePath}' does not exist!");
                return; 
            }

            DialogueSystem.Instance.Say(lines, filePath);
        }
    }
}