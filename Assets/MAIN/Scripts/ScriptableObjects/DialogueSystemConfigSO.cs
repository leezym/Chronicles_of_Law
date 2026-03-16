using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CHARACTERS;

namespace DIALOGUE
{
    [CreateAssetMenu(fileName = "Dialogue System Configuration Asset", menuName = "Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfigSO : ScriptableObject
    {
        public CharacterConfigSO characterConfigAsset;
        public Color defaultTextColor;
        public TMP_FontAsset defaultFont;
        public float defaultNameFontSize;
        public float defaultDialogueFontSize;
    }
}