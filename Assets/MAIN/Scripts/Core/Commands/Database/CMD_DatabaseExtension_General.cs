using System;
using System.Collections;
using System.Collections.Generic;
using COMMANDS;
using DIALOGUE;
using GAME;
using UnityEngine;
using UnityEngine.UI;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General : CMD_DatabaseExtension
    {

        //private static readonly string PARAM_FILEPATH = "archivo";
        //private static readonly string PARAM_ENQUEUE = "cola";
        private static readonly string PARAM_ITEM = "-nombre";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("mostrardialogo", new Func<IEnumerator>(ShowDialogueBox));
            database.AddCommand("ocultardialogo", new Func<IEnumerator>(HideDialogueBox));

            database.AddCommand("mostrarinterfaz", new Func<IEnumerator>(ShowDialogueSystem));
            database.AddCommand("ocultarinterfaz", new Func<IEnumerator>(HideDialogueSystem));

            database.AddCommand("cargararchivo", new Action<string/*string[]*/>(LoadNewDialogueFile)); //Cuando se hagan las interacciones sociales se integra si es necesario cambiar de escenas en las interacciones sociales (EP21 PART1)

            database.AddCommand("subirnivel", new Action(SetCurrentLevel));
        }

        private static void LoadNewDialogueFile(string data)
        {
            string fileName = data;
            //bool enqueue = false;

            //parameters.TryGetValue(PARAM_FILEPATH, out fileName);
            //parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

            Debug.Log(FilePaths.resources_dialogueFiles+fileName);
            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, fileName);
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if(file == null)
            {
                Debug.LogWarning($"File '{filePath}' could not be loaded from dialogue files. Please ensure it exists within Resources!");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);

            /*if(enqueue)
                DialogueSystem.Instance.conversationManager.Enqueue(newConversation);
            else*/
                DialogueSystem.Instance.conversationManager.StartConversation(newConversation);
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

        private static void SetCurrentLevel()
        {
            CasesManager.Instance.LevelChanged();            
        }
    }
}