using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class WorldEffect : IDataPoint
    {
        public GameObject Prefab;

        public Vector3 Position;

        public bool KartIsParent;
        public bool UseKartPosition;
        public bool UseKartRotation;

        public bool UseIdentityRotation;
        public Vector3 Rotation;
    }
}