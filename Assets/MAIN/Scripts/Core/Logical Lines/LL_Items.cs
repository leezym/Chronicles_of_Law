using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;

namespace DIALOGUE.LogicalLines
{
    public class LL_Items : ILogicalLine
    {
        public string keyword => "item";

        public IEnumerator Execute(DIALOGUE_LINE line)
        { 
            string name = line.dialogueData.rawData;

            //Try to get the name or path to the sprite
            string filePath = FilePaths.GetPathToResource(FilePaths.resources_itemsFiles, TestDialogueFiles.Instance.fileToRead.name+"/"+name);

            Sprite sprite = Resources.Load<Sprite>(filePath);

            if(sprite == null)
            {
                Debug.LogError($"Could not load sprite from path '{filePath}.' Please ensure it exists within Resources!");
                yield return null;
            }
            
            ItemsPanel itemsPanel = ItemsPanel.Instance;
            FolderPanel folderPanel = FolderPanel.Instance;
            
            itemsPanel.Show(sprite);
            folderPanel.CreateItemPrefab(sprite, name);

            while(itemsPanel.isWaitingOnUserChoice)
                yield return null;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }
}