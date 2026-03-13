using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using DIALOGUE;
using GAME;

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

            database.AddCommand("cargarcaso", new Action<string[]>(LoadNewCaseFile));
            database.AddCommand("cargarminijuego", new Func<IEnumerator>(LoadNewGame));

            database.AddCommand("subirnivelcaso", new Action(SetCurrentCaseLevel));
            database.AddCommand("subirnivel", new Action(SetCurrentLevel));
            //database.AddCommand("subirniveljuego", new Action(SetCurrentGameLevel));
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

        private static IEnumerator LoadNewGame()
        {
            DialogueSystem.Instance.conversationManager.PauseConversation();

            string fullPath = AssetDatabase.GetAssetPath(MinigamesManager.Instance.minigamesInGame[GameManager.Instance.GetCurrentGameLevel()].game);

            int resourcesIndex = fullPath.IndexOf("Resources/");
            string relativePath = fullPath.Substring(resourcesIndex + 10); // "Resources/" tiene tamaño 10

            string filePath = Path.ChangeExtension(relativePath, null);        
            GameObject prefab = Resources.Load<GameObject>(filePath);

            if(prefab == null)
            {
                Debug.LogWarning($"Prefab '{filePath}' could not be loaded from game files. Please ensure it exists within Resources!");
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            GameObject obj = UnityEngine.Object.Instantiate(prefab, MinigamesManager.Instance.layoutGroup.transform);
            obj.transform.SetAsFirstSibling();
            obj.name = prefab.name;
        }

        private static void SetCurrentCaseLevel()
        {
            CasesManager.Instance.LevelChanged();          
        }

        private static void SetCurrentLevel()
        {
            int level = GameManager.Instance.GetCurrentLevel() + 1;
            GameManager.Instance.SetCurrentLevel(level);
            TestDialogueFiles.Instance.StartCinematic();
        }

        /*private static void SetCurrentGameLevel()
        {
            MinigamesManager.Instance.LevelChanged();
        }*/
    }
}