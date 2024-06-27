using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class PhysicStats : IDataPoint, IStats
    {
        [Range(0f, 20f), Tooltip("Coefficient used to reorient the kart in the air. The higher the number, the faster the kart will readjust itself along the horizontal plane")]
        public float AirborneReorientationCoefficient = 1f;

        [Tooltip("Which layers the wheels will detect")]
        public LayerMask GroundLayers = Physics.DefaultRaycastLayers;


        public void SetStats(IStats newStats)
        {
            if (newStats is PhysicStats)
            {
                PhysicStats stats = (PhysicStats)newStats;

                AirborneReorientationCoefficient = stats.AirborneReorientationCoefficient;
                GroundLayers = stats.GroundLayers;

                return;
            }

            Debug.LogWarning($"Cannot set {this} to {newStats}");
        }

        public void AddStats(IStats otherStats)
        {
            if (otherStats is PhysicStats)
            {
                PhysicStats stats = (PhysicStats)otherStats;

                AirborneReorientationCoefficient += stats.AirborneReorientationCoefficient;
                GroundLayers |= stats.GroundLayers;

                return;
            }

            Debug.LogWarning($"Cannot add {otherStats} to {this}");
        }

        public void ClampStats()
        {
            AirborneReorientationCoefficient = Mathf.Clamp(AirborneReorientationCoefficient, 0f, 20f);
        }
    }
}