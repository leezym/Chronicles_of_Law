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
        public Color nameColor;
        public Color dialogueColor;
        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public float nameFontSize;
        public float dialogueFontSize;

        [SerializedDictionary("Path / ID", "Sprite")]
        public SerializedDictionary<string, Sprite> sprites = new SerializedDictionary<string, Sprite>();

        private static Color defaultColor => DialogueSystem.Instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont => DialogueSystem.Instance.config.defaultFont;

        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();

            result.name = name;
            result.characterType = characterType;
            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;
            result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
            result.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);

            result.dialogueFontSize = dialogueFontSize;
            result.nameFontSize = nameFontSize;

            result.sprites = sprites;

            return result;
        }

        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();

                result.name = "";
                result.characterType = Character.CharacterType.Text;
                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;
                result.nameColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                result.dialogueColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                result.dialogueFontSize = DialogueSystem.Instance.config.defaultDialogueFontSize;
                result.nameFontSize = DialogueSystem.Instance.config.defaultNameFontSize;
                result.sprites = new SerializedDictionary<string, Sprite>();


                return result;
            }
        }
    }
}