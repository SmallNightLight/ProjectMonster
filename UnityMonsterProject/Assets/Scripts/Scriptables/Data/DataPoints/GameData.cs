using ScriptableArchitecture.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class GameData : IDataPoint
    {
        public MapData Map;
        public int PlayerCount;
        public List<CharacterData> CharacterDatas;
        public GameState State;

        [Header("Default values")]
        public MapDataReference DefaultMap;
        public List<CharacterDataReference> DefaultCharacterDatas;

        public void Reset()
        {
            PlayerCount = 2;
            State = GameState.StartCinematic;

            Map = DefaultMap.Value;
            CharacterDatas = DefaultCharacterDatas.Select(data => data.Value).ToList();
        }

        public void ChangeState(GameState state)
        {
            State = state;
        }

        public bool TryGetCharacterData(int index, out CharacterData characterData)
        {
            if (CharacterDatas == null || CharacterDatas.Count <= index)
            {
                characterData = null;
                return false;
            }

            characterData = CharacterDatas[index];
            return true;
        }
    }

    public enum GameState
    {
        UI,
        StartCinematic,
        CountDown,
        Gameplay,
        EndCinematic,
        Score
    }
}