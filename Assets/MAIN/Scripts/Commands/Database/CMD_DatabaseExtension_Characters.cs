using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Characters : CMD_DatabaseExtension
    {
        private static string PARAM_XPOS = "-x";
        private static string PARAM_YPOS = "-y";
        private static string PARAM_SPEED = "-vel";
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("crear", new Action<string[]>(CreateCharacter));
            database.AddCommand("mover", new Func<string[], IEnumerator>(MoveCharacter));
            database.AddCommand("mostrar", new Func<string[], IEnumerator>(ShowAll));
            database.AddCommand("ocultar", new Func<string[], IEnumerator>(HideAll));
        }

        public static void CreateCharacter(string[] data)
        {
            string characterName = data[0];
            
            Character character = CharacterManager.Instance.CreateCharacter(characterName);
            
            if(character != null)
                character.Show();
        }

        public static IEnumerator MoveCharacter(string[] data)
        {
            string characterName = data[0];
            Character character = CharacterManager.Instance.GetCharacter(characterName);

            if(character == null)
                yield break;

            float x, y;
            float speed;

            var parameters = ConvertDataToParameters(data);

            //try to get the x and y axis position
            parameters.TryGetValue(PARAM_XPOS, out x);
            parameters.TryGetValue(PARAM_YPOS, out y);

            //try to get the speed
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 2);

            yield return character.MoveToPosition(new Vector2(x, y), speed);
        }

        public static IEnumerator ShowAll(string[] data)
        {
            List<Character> characters = new List<Character>();

            foreach(string s in data)
            {
                Character character = CharacterManager.Instance.GetCharacter(s, createIfDoesNotExist: false);
                if(character != null)
                    characters.Add(character);
            }

            if(characters.Count == 0)
                yield break;

            foreach(Character character in characters)
                character.Show();
        }

        public static IEnumerator HideAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            foreach(string s in data)
            {
                Character character = CharacterManager.Instance.GetCharacter(s, createIfDoesNotExist: false);
                if(character != null)
                    characters.Add(character);
            }

            if(characters.Count == 0)
                yield break;

            foreach(Character character in characters)
                character.Hide();
        }
    }
}