using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DIALOGUE;
using HISTORY;
using GAME;

[System.Serializable]
public class VNGameSave
{
    public static VNGameSave activeFile = null;

    public const string FILE_TYPE = ".vns";
    public const bool ENCRYPT = false; //Change if you want encrypt or not your files
    public const string TEMP_NAME = "DATA";

    public string filePath = $"{FilePaths.gameSaves}{TEMP_NAME}{FILE_TYPE}";

    public string[] activeConversations;

    public HistoryState activeState;
    public string PathReplace (string name) {return filePath.Replace(TEMP_NAME, name);}

    public static VNGameSave Load(string filePath, bool activateOnLoad = false)
    {
        VNGameSave save = FileManager.Load<VNGameSave>(filePath, ENCRYPT);

        activeFile = save;

        if(activateOnLoad)        
            save.Activate();
        
        return save;
    }
    public void Save()
    {
        activeState = HistoryState.Capture();
        activeConversations = GetConversationData();

        string saveJSON = JsonUtility.ToJson(this);
        FileManager.Save(filePath, saveJSON, ENCRYPT);
    }

    public void Activate()
    {
        if(activeState != null)
            activeState.Load();
        
        HistoryManager.Instance.history = activeState;
        SetConversationData();

        DialogueSystem.Instance.prompt.Hide();
    }

    private string[] GetConversationData()
    {
        List<string> retData = new List<string>();
        var conversations = DialogueSystem.Instance.conversationManager.GetConversationQueue();

        for(int i = 0; i < conversations.Length; i++)
        {
            var conversation = conversations[i];
            string data = "";

            if(conversation.file != string.Empty)
            {
                var compressedData = new VN_ConversationDataCompressed();

                //filePath = PathReplace(conversation.file); // Se comenta porque solo se necesita el ultimo nivel guardado, por ahora

                compressedData.fileName = conversation.file;
                compressedData.progress = conversation.GetProgress();
                compressedData.startIndex = conversation.fileStartIndex;
                compressedData.endIndex = conversation.fileEndIndex;
                data = JsonUtility.ToJson(compressedData);
            }
            else
            {
                var fullData = new VN_ConversationData();
                fullData.conversation = conversation.GetLines();
                fullData.progress = conversation.GetProgress();
                data = JsonUtility.ToJson(fullData);
            }

            retData.Add(data);
        }
        
        return retData.ToArray();
    }

    private void SetConversationData()
    {
        for(int i= 0; i < activeConversations.Length; i++)
        {
            try
            {
                string data = activeConversations[i];
                Conversation conversation = null;

                var fullData = JsonUtility.FromJson<VN_ConversationData>(data);
                if(fullData != null && fullData.conversation != null && fullData.conversation.Count > 0)
                {
                    conversation = new Conversation(fullData.conversation, fullData.progress);
                }
                else
                {
                    var compressedData = JsonUtility.FromJson<VN_ConversationDataCompressed>(data);
                    if(compressedData != null && compressedData.fileName != string.Empty)
                    {
                        TextAsset file = Resources.Load<TextAsset>(compressedData.fileName);

                        int count = compressedData.endIndex - compressedData.startIndex;

                        List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count + 1).ToList();

                        conversation = new Conversation(lines,compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endIndex);
                    }
                    else
                    {
                        Debug.LogError($"Unknown conversation format! Unable to reload conversation from VNGameSave using data '{data}'");
                    }
                }

                if(conversation != null && conversation.GetLines().Count > 0)
                {
                    if(i == 0)
                        DialogueSystem.Instance.conversationManager.StartConversation(conversation);
                    else
                        DialogueSystem.Instance.conversationManager.Enqueue(conversation);
                }
            }
            catch(System.Exception e)
            {
                Debug.LogError($"Encounteres error while extracting saved conversation data! {e}");
                continue;
            }
        }
    }
}
