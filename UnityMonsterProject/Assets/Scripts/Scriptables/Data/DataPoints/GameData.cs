using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class GameData : IDataPoint
    {
        public int Map;
        public int PlayerCount;
        public GameState State;

        public enum GameState 
        {
            StartCinematic,
            CountDown,
            Gameplay,
            EndCinematic,
            Score
        }

        public void Reset()
        {
            Map = 0;
            PlayerCount = 2;
            State = GameState.StartCinematic;
        }

        public void ChangeState(GameState state)
        {
            State = state;
        }
    }
}