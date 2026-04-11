using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DIALOGUE;
using AYellowpaper.SerializedCollections;

namespace CHARACTERS
{
    [System.Serializable]
    public class CharacterConfigData
    {
        public string name;
        public Character.CharacterType characterType;
        public GameObject prefab;        
        [SerializedDictionary("Path / ID", "Sprite")]
        public SerializedDictionary<string, Sprite> sprites = new SerializedDictionary<string, Sprite>();

        [HideInInspector]
        public TMP_FontAsset nameFont, dialogueFont;
        [HideInInspector]
        public Color nameColor, dialogueColor;
        [HideInInspector]
        public float nameFontSize, dialogueFontSize;
        private static Color defaultNameColor => DialogueSystem.Instance.config.defaultNameColor;
        private static Color defaultDialogueColor => DialogueSystem.Instance.config.defaultDialogueColor;
        private static TMP_FontAsset defaultFont => DialogueSystem.Instance.config.defaultFont;
        private static float defaultDialogueFontSize => DialogueSystem.Instance.config.defaultDialogueFontSize;
        private static float defaultNameFontSize => DialogueSystem.Instance.config.defaultNameFontSize;

        public CharacterConfigData(string name, Character.CharacterType characterType, TMP_FontAsset nameFont, TMP_FontAsset dialogueFont,
            Color nameColor, Color dialogueColor, float nameFontSize, float dialogueFontSize, GameObject prefab, SerializedDictionary<string, Sprite> sprites)
        {
            this.name = name;
            this.characterType = characterType;
            this.nameFont = nameFont;
            this.dialogueFont = dialogueFont;
            this.nameColor = nameColor;
            this.dialogueColor = dialogueColor;
            this.dialogueFontSize = dialogueFontSize;
            this.nameFontSize = nameFontSize;
            this.prefab = prefab;
            this.sprites = sprites;
        }

        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData(name, characterType, defaultFont, defaultFont,
                new Color(defaultNameColor.r, defaultNameColor.g, defaultNameColor.b, defaultNameColor.a), new Color(defaultDialogueColor.r, defaultDialogueColor.g, defaultDialogueColor.b, defaultDialogueColor.a),
                defaultDialogueFontSize, defaultNameFontSize, prefab, sprites);

            return result;
        }

        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData("", Character.CharacterType.Text, defaultFont, defaultFont,
                new Color(defaultNameColor.r, defaultNameColor.g, defaultNameColor.b, defaultNameColor.a), new Color(defaultDialogueColor.r, defaultDialogueColor.g, defaultDialogueColor.b, defaultDialogueColor.a),
                defaultDialogueFontSize, defaultNameFontSize, null, new SerializedDictionary<string, Sprite>());

                return result;
            }
        }
    }
}