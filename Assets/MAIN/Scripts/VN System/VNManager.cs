using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
using GAME;

namespace VISUALNOVEL
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        /*public void StartLevel()
        {
            TextAsset file = CasesManager.Instance.casesInGame[GameManager.Instance.GetCurrentCaseLevel()].cases.fileToRead;
            LoadFile(file);
        }*/
        
        public void StartF()
        {
            GameManager.Instance.SetGenderF();
            StartCinematic();
        }

        public void StartM()
        {
            GameManager.Instance.SetGenderM();
            StartCinematic();
        }
        public void StartCinematic()
        {        
            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, $"Nivel.{GameManager.Instance.GetCurrentLevel()}");
            LoadFile(filePath);
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

        public void LoadFile(TextAsset file)
        {
            List<string> lines = new List<string>();
            
            try
            {
                lines = FileManager.ReadTextAsset(file);
            }
            catch
            {
                Debug.LogError($"Dialogue file does not exist!");
                return; 
            }

            DialogueSystem.Instance.Say(lines);
        }
    }
}