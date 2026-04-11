using System.Collections;
using System.Collections.Generic;
using GAME;
using UnityEngine;

namespace HISTORY
{
    [System.Serializable]
    public class HistoryState
    {
        public DialogueData dialogue;
        public List<CharacterData> characters;
        public List<AudioData> audio;
        public List<GraphicData> graphics;
        public List<CasesData> casesData;
        public List<CasesInGame> casesInGame;
        public List<MinigamesData> minigamesData;
        public List<MinigamesData> minigamesInGame;
        public GameData game;

        public static HistoryState Capture()
        {
            HistoryState state = new HistoryState();
            state.dialogue = DialogueData.Capture(); // solo si se necesita arrancar desde el ultimo punto (checkpoint) o se necesita un historial
            state.characters = CharacterData.Capture();
            state.audio = AudioData.Capture();
            state.graphics = GraphicData.Capture();
            state.game = GameData.Capture();
            state.casesData = CasesData.Capture();
            state.casesInGame = CasesInGame.Capture();
            state.minigamesData = MinigamesData.Capture();
            state.minigamesInGame = MinigamesData.CaptureInGame();

            return state;
        }

        public void Load()
        {
            DialogueData.Apply(dialogue);
            CharacterData.Apply(characters);
            AudioData.Apply(audio);
            GraphicData.Apply(graphics);
            GameData.Apply(game);
            CasesData.Apply(casesData);
            CasesInGame.Apply(casesInGame);
            MinigamesData.Apply(minigamesData);
            MinigamesData.ApplyInGame(minigamesInGame);
            
            CasesManager.Instance.AssignCasesToButtons();
        }
    }
}