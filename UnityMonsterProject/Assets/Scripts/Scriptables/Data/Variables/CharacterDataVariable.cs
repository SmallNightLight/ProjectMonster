using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "PlayerDataVariable", menuName = "Scriptables/Variables/PlayerData")]
    public class CharacterDataVariable : Variable<CharacterData>
    {
    }
}