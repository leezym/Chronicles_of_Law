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
        [SerializeField]
        private VNGameSave save;
        public VNMenuManager vmm;

        public Button continueButton;
        public Button saveButton;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            VNGameSave.activeFile = new VNGameSave();
        }

        public void StartGame()
        {
            if (continueButton.interactable)
                NotificationsManager.Instance.QuestionNotification(
                    "¿Está seguro de iniciar nueva partida? Perderá el progreso de la partida anterior.",
                    () => {
                        File.Delete($"{FilePaths.gameSaves}{VNGameSave.TEMP_NAME}{VNGameSave.FILE_TYPE}");
                        InitializeAndStart();
                    });
            else
                InitializeAndStart();
        }

        private void InitializeAndStart()
        {
            CasesManager.Instance.InitializeCases();
            MinigamesManager.Instance.InitializeMinigames();
            GameManager.Instance.InitializeGame();
            vmm.OpenCharacterMenu();
        }

        public void ContinueGame()
        {
            Load();
        }

        public void GenderSelection(string gender)
        {
            StartGender(gender == "M" ? Gender.Male : Gender.Female);
            StartLevel();   
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
                Debug.LogWarning("There are not a selected option (F or M).");
            }
        }
        public void StartLevel()
        { 
            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, $"Nivel.{GameManager.Instance.GetCurrentLevel()}");
            LoadFile(filePath);
        }
        
        public void Save()
        {
            VNGameSave.activeFile.Save();
            NotificationsManager.Instance.WarningNotification("¡Partida guardada!");
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

        public void OpenMainmMenuFromPause()
        {
            NotificationsManager.Instance.QuestionNotification("¿Está seguro de salir del juego? ¡Recuerda guardar tu progreso!", vmm.OpenMainMenu);
        }
    }
}