using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System.IO;
using DIALOGUE;
using CHARACTERS;
using GAME;
using UnityEngine.UI;

namespace VISUALNOVEL
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { get; private set; }
        static string HOME_MUSIC = "HOME_Shadow_of_the_Verdict";
        VNGameSave save;

        public Button continueButton;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            VNGameSave.activeFile = new VNGameSave();
            AudioClip audio = Resources.Load<AudioClip>(FilePaths.resources_music + HOME_MUSIC);
            AudioManager.Instance.PlayTrack(audio, AudioBus.Music);
            continueButton.interactable = File.Exists($"{FilePaths.gameSaves}{VNGameSave.TEMP_NAME}{VNGameSave.FILE_TYPE}");
        }

        public void StartGame()
        {
            CasesManager.Instance.InitializeCases();
            MinigamesManager.Instance.InitializeMinigames();
            GameManager.Instance.InitializeGame();
        }

        public void ContinueGame()
        {
            Load();
            AudioManager.Instance.StopTrack(HOME_MUSIC);
        }

        public void GenderSelection(string gender)
        {
            StartGender(gender == "M" ? Gender.Male : Gender.Female);
            StartLevel();
            AudioManager.Instance.StopTrack(HOME_MUSIC);
            
        }

        void StartGender(Gender gender)
        {
            if (gender == Gender.Male)
            {
                GameManager.Instance.SetGenderM();
                CharacterManager.Instance.SetCharacterConfigMasculine("Avatar");
            }
            else if (gender == Gender.Female)
            {
                GameManager.Instance.SetGenderF();
                CharacterManager.Instance.SetCharacterConfigFemenine("Avatar");
            }
            else
            {
                Debug.LogWarning("There are not a selected option ( F or M).");
            }
        }
        public void StartLevel()
        { 
            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, $"Nivel.{GameManager.Instance.GetCurrentLevel()}");
            LoadFile(filePath);
        }

        public void MainMenu()
        {
            AudioClip audio = Resources.Load<AudioClip>(FilePaths.resources_music + HOME_MUSIC);
            AudioManager.Instance.PlayTrack(audio, AudioBus.Music);
        }

        public void Save()
        {
            VNGameSave.activeFile.Save();
        }

        public void Load()
        {
            try
            {
                save = VNGameSave.Load($"{FilePaths.gameSaves}{VNGameSave.TEMP_NAME}{VNGameSave.FILE_TYPE}", activateOnLoad: true);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Do something because we found an error. {e.ToString()}");
            }
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