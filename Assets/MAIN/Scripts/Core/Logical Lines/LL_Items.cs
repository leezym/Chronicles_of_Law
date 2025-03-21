using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using GAME;
using System;

namespace DIALOGUE.LogicalLines
{
    public class LL_Items : ILogicalLine
    {
        public string keyword => "item";

        public IEnumerator Execute(DIALOGUE_LINE line)
        { 
            var gm = GameManager.Instance;
            string spriteName = line.dialogueData.rawData;
            string folderName = CasesManager.Instance.casesInGame[gm.GetCurrentLevel()].cases.fileToRead.name;
            string caseName = CasesManager.Instance.casesInGame[gm.GetCurrentLevel()].cases.name;

           if(CasesManager.Instance.casesInGame[gm.GetCurrentLevel()].cases.fileToRead == null || string.IsNullOrWhiteSpace(CasesManager.Instance.casesInGame[gm.GetCurrentLevel()].cases.fileToRead.text))
            {
                Debug.LogError("Could not load case from. Please ensure it exists within Cases Data!");
                yield return null;
            }

            //Try to get the name or path to the sprite
            string filePath = FilePaths.GetPathToResource(FilePaths.resources_itemsFiles, folderName+"/"+spriteName);

            Sprite sprite = Resources.Load<Sprite>(filePath);

            if(sprite == null)
            {
                Debug.LogError($"Could not load sprite from path '{filePath}'. Please ensure it exists within Resources!");
                yield return null;
            }
            
            ItemsPanel itemsPanel = ItemsPanel.Instance;
            FolderPanel folderPanel = FolderPanel.Instance;
            
            itemsPanel.Show(sprite);

            if (!gm.items.Any(item => item.nameItem == spriteName))
            {
                folderPanel.CreateItemPrefab(sprite, spriteName);
                folderPanel.AddItemPrefab(sprite, spriteName, caseName); 
            }
            else
            {
                Debug.LogWarning($"A item called '{spriteName}' already exists. Did not create the item");
            }            

            while(itemsPanel.isWaitingOnUserChoice)
                yield return null;
        }

        public bool Matches(DIALOGUE_LINE line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }
}