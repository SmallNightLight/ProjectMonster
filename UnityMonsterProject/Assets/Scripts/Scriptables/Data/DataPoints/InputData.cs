using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class InputData : IAssignment<InputData>
    {
        public bool IsAccelerating;
        public bool IsBraking;
        public float SteerInput;
        public bool IsTricking;
        public bool AbilityBoost;
        public bool Ability1;

        //UI
        public bool Press;
        public bool Back;
        public bool MoveUp;
        public bool MoveDown;
        public bool MoveRight;
        public bool MoveLeft;

        public InputData Copy()
        {
            return new InputData
            {
                IsAccelerating = IsAccelerating,
                IsBraking = IsBraking,
                SteerInput = SteerInput,
                IsTricking = IsTricking,
                AbilityBoost = AbilityBoost,
                Ability1 = Ability1,

                Press = Press,
                Back = Back,
                MoveUp = MoveUp,
                MoveDown = MoveDown,
                MoveRight = MoveRight,
                MoveLeft = MoveLeft
            };
        }
    }
}