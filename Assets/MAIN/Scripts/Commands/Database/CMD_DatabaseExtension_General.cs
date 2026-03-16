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

            database.AddCommand("cargarcaso", new Action<string[]>(LoadNewCaseFile));
            database.AddCommand("cargarminijuego", new Func<IEnumerator>(LoadNewGame));

            database.AddCommand("subirnivelcaso", new Action(SetCurrentCaseLevel));
            database.AddCommand("subirnivel", new Action(SetCurrentLevel));
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

            TextAsset file = CasesManager.Instance.casesInGame[GameManager.Instance.GetCurrentCaseLevel()].cases.fileToRead;

            if(file == null)
            {
                Debug.LogWarning($"File '{file}' could not be loaded from dialogue files. Please ensure it exists!");
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

            GameObject prefab = MinigamesManager.Instance.minigamesInGame[GameManager.Instance.GetCurrentGameLevel()].game;

            if(prefab == null)
            {
                Debug.LogWarning($"Prefab '{prefab}' could not be loaded from game files. Please ensure it exists!");
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            MinigamesManager.Instance.StartGame(prefab);
        }

        private static void SetCurrentCaseLevel()
        {
            CasesManager.Instance.LevelChanged();          
        }

        private static void SetCurrentLevel()
        {
            GameManager.Instance.SetCurrentLevel(GameManager.Instance.GetCurrentLevel());
            VNManager.Instance.StartCinematic();
        }
    }
}