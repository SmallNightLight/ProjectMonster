using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class CharacterData : IDataPoint
    {
        public GameObject CharacterPrefab;
        public GameObject KartPrefab;

        public Color PrimaryColor;
        public Color SecondaryColor;

        public AbilityDataReference MainAbility;
        public AbilityDataReference BoostAbility;

        public Sprite CharacterIcon;
    }
}