using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "MovementStatsVariable", menuName = "Scriptables/Variables/MovementStats")]
    public class MovementStatsVariable : Variable<MovementStats>, IStatsVariable
    {
    }
}