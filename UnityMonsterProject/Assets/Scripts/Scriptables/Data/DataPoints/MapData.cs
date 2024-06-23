using ScriptableArchitecture.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class MapData : IDataPoint
    {
        public List<Vector3> PlayerSpawnPositions;
        public List<Vector3> PlayerSpawnRotations;
        public GameObject MapPrefab;
        public GameObject SplinePrefab;
        public int TotalLaps;
        public GameObject MiniMapPrefab;
    }
}