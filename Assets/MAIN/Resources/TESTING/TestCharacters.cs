using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        void Start()
        {
            //Character Lila = CharacterManager.Instance.CreateCharacter("Lila");
            //Character Girl = CharacterManager.Instance.CreateCharacter("Girl");
            StartCoroutine(Test());
        }

        IEnumerator Test() //SetPosition, Show, Move if u want, GetSprite and Setsprite if u want
        {
            Character_Sprite Girl = CharacterManager.Instance.CreateCharacter("Girl") as Character_Sprite;

            Girl.SetPosition(Vector2.zero);
            Sprite Girl_faceSprite = Girl.GetSprite("Enojado");
            yield return new WaitForSeconds(1f);
            Girl.TransitionSprite(Girl_faceSprite, 1);

            yield return new WaitForSeconds(1f);

            Character_Sprite Lila = CharacterManager.Instance.CreateCharacter("Lila") as Character_Sprite;
            Sprite Lila_bodySprite = Lila.GetSprite("Feliz");
            yield return Lila.MoveToPosition(new Vector2(0.3f, 0));
            yield return Lila.TransitionSprite(Lila_bodySprite); // Or SetSprite without transitions

            Girl.TransitionSprite(Girl.GetSprite("Sonrojado"), 1);
            
            yield return null;
        }
    }
}