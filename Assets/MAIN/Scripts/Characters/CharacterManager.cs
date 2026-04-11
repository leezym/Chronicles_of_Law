using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using DIALOGUE;

namespace CHARACTERS
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }
        public List<Character> allCharacters { get; set; } = new List<Character>();

        private CharacterConfigSO config => DialogueSystem.Instance.config.characterConfigAsset;

        public GameObject masculinePrefab;
        public GameObject femeninePrefab;

        public SerializedDictionary<string, Sprite> masculineSprites = new SerializedDictionary<string, Sprite>(); 
        public SerializedDictionary<string, Sprite> femenineSprites = new SerializedDictionary<string, Sprite>(); 

        private const string CHARACTER_NAME_ID = "<charname>";
        public string characterRootPathFormat => $"Characters/{CHARACTER_NAME_ID}";
        public string characterPrefabNameFormat => $"Character - [{CHARACTER_NAME_ID}]";

        [SerializeField] private RectTransform _characterPanel = null;
        public RectTransform characterPanel => _characterPanel;

        private void Awake()
        {
            Instance = this;
        }

        public CharacterConfigData[] GetCharacterConfigArray()
        {
            return config.characters;
        }

        public CharacterConfigData GetCharacterConfig(string characterName)
        {
            return config.GetConfig(characterName);
        }

        public void SetCharacterConfig(string characterName, GameObject prefab, SerializedDictionary<string, Sprite> sprites)
        {
            config.SetConfig(characterName, prefab, sprites);
        }

        public void SetCharacterConfigFemenine(string characterName)
        {
            config.SetConfig(characterName, femeninePrefab, femenineSprites);
        }

        public void SetCharacterConfigMasculine(string characterName)
        {
            config.SetConfig(characterName, masculinePrefab, masculineSprites);
        }

        public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
        {
            if(allCharacters.Exists(character => character.name.ToLower() == characterName.ToLower()))
                return allCharacters.Find(character => character.name.ToLower() == characterName.ToLower());
            else if(createIfDoesNotExist)
                return CreateCharacter(characterName);

            return null;
        }

        public Character CreateCharacter(string characterName, bool revealAfterCreation = false)
        {            
            if(allCharacters.Exists(character => character.name.ToLower() == characterName.ToLower()))
            {
                Debug.LogWarning($"A Character called '{characterName}' already exists. Did not create the character");
                return null;
            }
            
            CHARACTER_INFO info = GetCharacterInfo(characterName);
            Character character = CreateCharacterFromInfo(info);
            allCharacters.Add(character);

            if(revealAfterCreation)
                character.Show();

            return character;
        }

        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO result = new CHARACTER_INFO();

            result.name = characterName;
            result.config = config.GetConfig(characterName);

            return result;
        }

        public string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);

        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            CharacterConfigData config = info.config;

            switch(config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name, config);
                
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new Character_Sprite(info.name, config);
                
                default:
                    return null;
            }
        }

        private class CHARACTER_INFO
        {
            public string name = "";
            public string rootCharacterFolder = "";
            public CharacterConfigData config = null;
        }
    }
}