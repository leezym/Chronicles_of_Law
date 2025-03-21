using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

namespace HISTORY
{
    [System.Serializable]
    public class CharacterData
    { 
        public string characterName;
        public bool enabled;
        public Vector2 position;
        public CharacterConfigCache characterConfig;

        public string dataJSON;

        [System.Serializable]
        public class CharacterConfigCache
        {
            public string name;

            public Character.CharacterType characterType;

            public Color nameColor;
            public Color dialogueColor;

            public string nameFont;
            public string dialogueFont;

            public CharacterConfigCache(CharacterConfigData reference)
            {
                name = reference.name;
                characterType = reference.characterType;

                nameColor = reference.nameColor;
                dialogueColor = reference.dialogueColor;

                nameFont = FilePaths.resources_font + reference.nameFont.name;
                dialogueFont = FilePaths.resources_font + reference.dialogueFont.name;
            }            
        }

        public static List<CharacterData> Capture()
        {
            List<CharacterData> characters = new List<CharacterData>();

            foreach(Character character in CharacterManager.Instance.allCharacters)
            {
                if(!character.isVisible)
                    continue;
                
                CharacterData entry = new CharacterData();
                entry.characterName = character.name;
                entry.enabled = character.isVisible;
                entry.position = character.targetPosition;
                entry.characterConfig = new CharacterConfigCache(character.config);

                switch(character.config.characterType)
                {
                    case Character.CharacterType.Sprite:
                    case Character.CharacterType.SpriteSheet:
                        SpriteData sData = new SpriteData();
                        sData.layers = new List<SpriteData.LayerData>();

                        Character_Sprite sc = character as Character_Sprite;
                        foreach(var layer in sc.layers)
                        {
                            var layerData = new SpriteData.LayerData();
                            layerData.color = layer.renderer.color;
                            layerData.spriteName = layer.renderer.sprite.name;
                            sData.layers.Add(layerData);
                        }

                        entry.dataJSON = JsonUtility.ToJson(sData);
                        break;
                }

                 characters.Add(entry);
            }

            return characters;
        }

        public static void Apply(List<CharacterData> data)
        {
            List<string> cache = new List<string>();

            foreach(CharacterData characterData in data)
            {
                Character character = CharacterManager.Instance.GetCharacter(characterData.characterName, createIfDoesNotExist: true);
                character.name = characterData.characterName;
                character.SetPosition(characterData.position);
                character.isVisible = characterData.enabled;

                switch(character.config.characterType)
                {
                    case Character.CharacterType.Sprite:
                    case Character.CharacterType.SpriteSheet:
                        SpriteData sData = JsonUtility.FromJson<SpriteData>(characterData.dataJSON);
                        Character_Sprite sc = character as Character_Sprite;

                        for(int i = 0; i < sData.layers.Count; i++)
                        {
                            var layer = sData.layers[i];
                            if(sc.layers[i].renderer.sprite != null && sc.layers[i].renderer.sprite.name != layer.spriteName)
                            {
                                Sprite sprite = sc.GetSprite(layer.spriteName);
                                if(sprite != null)
                                    sc.SetSprite(sprite, i);
                                else
                                    Debug.LogWarning($"History State: could not load sprite '{layer.spriteName}'");
                            }
                        }
                        break;
                }

                cache.Add(character.name);
            }

            foreach(Character character in CharacterManager.Instance.allCharacters)
            {
                if(!cache.Contains(character.name))
                    character.isVisible = false;
            }
        }

        [System.Serializable]
        public class SpriteData
        {
            public List<LayerData> layers;

            [System.Serializable]
            public class LayerData
            {
                public string spriteName;
                public Color color;
            }
        }
    }
}
