using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "GameDataVariable", menuName = "Scriptables/Variables/GameData")]
    public class GameDataVariable : Variable<GameData>
    {
        [ContextMenu("Reset values")]
        public void ResetValues()
        {
            Value.Reset();
        }
    }
}