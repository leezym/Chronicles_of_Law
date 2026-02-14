using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using DIALOGUE;
using GAME;
using VISUALNOVEL;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General : CMD_DatabaseExtension
    {

        private static readonly string PARAM_FILEPATH = "-archivo";
        private static readonly string PARAM_ENQUEUE = "-cola";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("mostrardialogo", new Func<IEnumerator>(ShowDialogueBox));
            database.AddCommand("ocultardialogo", new Func<IEnumerator>(HideDialogueBox));

            database.AddCommand("mostrarinterfaz", new Func<IEnumerator>(ShowDialogueSystem));
            database.AddCommand("ocultarinterfaz", new Func<IEnumerator>(HideDialogueSystem));

            //database.AddCommand("cargararchivo", new Action<string[]>(LoadNewDialogueFile)); //Cuando se hagan las interacciones sociales se integra si es necesario cambiar de escenas en las interacciones sociales (EP21 PART1)
            database.AddCommand("cargarcaso", new Action<string[]>(LoadNewCaseFile)); //Cuando se hagan las interacciones sociales se integra si es necesario cambiar de escenas en las interacciones sociales (EP21 PART1)
            database.AddCommand("cargarminijuego", new Action(LoadNewGame));
            //database.AddCommand("cargarnivel", new Action<string[]>(LoadCurrentGameLevel));

            database.AddCommand("subirnivelcaso", new Action(SetCurrentCaseLevel));
            database.AddCommand("subirniveljuego", new Action(SetCurrentGameLevel));
        }

        private static IEnumerator ShowDialogueBox()
        {
            yield return DialogueSystem.Instance.dialogueContainer.Show();
        }

        private static IEnumerator HideDialogueBox()
        {
            yield return DialogueSystem.Instance.dialogueContainer.Hide();
        }

        private static IEnumerator ShowDialogueSystem()
        {
            yield return DialogueSystem.Instance.Show();
        }

        private static IEnumerator HideDialogueSystem()
        {
            yield return DialogueSystem.Instance.Hide();
        }

        /*private static void LoadNewDialogueFile(string[] data)
        {
            string fileName = string.Empty;
            bool enqueue = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_FILEPATH, out fileName);
            parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, fileName);
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if(file == null)
            {
                Debug.LogWarning($"File '{filePath}' could not be loaded from dialogue files. Please ensure it exists within Resources!");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);

            if(enqueue)
                DialogueSystem.Instance.conversationManager.Enqueue(newConversation);
            else
                DialogueSystem.Instance.conversationManager.StartConversation(newConversation);
        }*/

        private static void LoadNewCaseFile(string[] data)
        {
            bool enqueue = false;
            
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

            string fullPath = AssetDatabase.GetAssetPath(CasesManager.Instance.casesInGame[GameManager.Instance.GetCurrentCaseLevel()].cases.fileToRead);

            int resourcesIndex = fullPath.IndexOf("Resources/");
            string relativePath = fullPath.Substring(resourcesIndex + 10); // "Resources/" tiene tamaño 10

            string filePath = Path.ChangeExtension(relativePath, null);            
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if(file == null)
            {
                Debug.LogWarning($"File '{filePath}' could not be loaded from dialogue files. Please ensure it exists within Resources!");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);          

            if(enqueue)
                DialogueSystem.Instance.conversationManager.Enqueue(newConversation);
            else if (DialogueSystem.Instance.isRunningConversation)
                DialogueSystem.Instance.conversationManager.EnqueuePriority(newConversation);
            else
                DialogueSystem.Instance.conversationManager.StartConversation(newConversation);
        }

        private static void LoadNewGame()
        {
            Debug.Log("minijuego");
        }

        /*private static void LoadCurrentGameLevel(string[] data)
        {
            bool enqueue = false;
            
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, Nivel.{GameManager.Instance.GetCurrentGameLevel()}");
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if(file == null)
            {
                Debug.LogWarning($"File '{filePath}' could not be loaded from dialogue files. Please ensure it exists within Resources!");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);

            if(enqueue)
                DialogueSystem.Instance.conversationManager.Enqueue(newConversation);
            else
                DialogueSystem.Instance.conversationManager.StartConversation(newConversation);
        }*/

        private static void SetCurrentCaseLevel()
        {
            CasesManager.Instance.LevelChanged();            
        }

        private static void SetCurrentGameLevel()
        {
            GameManager gm = GameManager.Instance;
            int level = gm.GetCurrentGameLevel() + 1;
            
            gm.SetCurrentGameLevel(level);
        }
    }
}