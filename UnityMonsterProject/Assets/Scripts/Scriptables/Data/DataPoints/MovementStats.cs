using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class MovementStats : IDataPoint, IStats
    {
        [Tooltip("Top speed when moving forward")]
        public float TopSpeed;

        [Tooltip("How quickly the kart reaches top speed")]
        public float Acceleration;

        [Min(0.001f), Tooltip("Top speed attainable when moving backward")]
        public float ReverseSpeed;

        [Tooltip("How quickly the kart reaches top speed, when moving backward")]
        public float ReverseAcceleration;

        [Range(0.2f, 1f), Tooltip("How quickly the kart starts accelerating from 0. A higher number means it accelerates faster sooner")]
        public float AccelerationCurve;

        [Tooltip("How quickly the kart slows down when the brake is applied")]
        public float Braking;

        [Tooltip("How quickly the kart will reach a full stop when no inputs are made")]
        public float CoastingDrag;

        [Range(0f, 1f), Tooltip("The amount of side-to-side friction")]
        public float Grip;

        [Tooltip("How tightly the kart can turn left or right")]
        public float Steer;

        [Tooltip("Additional gravity for when the kart is in the air")]
        public float AddedGravity;


        public void SetStats(IStats newStats)
        {
            if (newStats is MovementStats)
            {
                MovementStats stats = (MovementStats)newStats;

                TopSpeed = stats.TopSpeed;
                Acceleration = stats.Acceleration;
                ReverseSpeed = stats.ReverseSpeed;
                ReverseAcceleration = stats.ReverseAcceleration;
                AccelerationCurve = stats.AccelerationCurve;
                Braking = stats.Braking;
                CoastingDrag = stats.CoastingDrag;
                Grip = stats.Grip;
                Steer = stats.Steer;
                AddedGravity = stats.AddedGravity;

                return;
            }

            Debug.LogWarning($"Cannot set {this} to {newStats}");
        }

        public void AddStats(IStats otherStats)
        {
            if (otherStats is MovementStats)
            {
                MovementStats stats = (MovementStats)otherStats;

                TopSpeed += stats.TopSpeed;
                Acceleration += stats.Acceleration;
                ReverseSpeed += stats.ReverseSpeed;
                ReverseAcceleration += stats.ReverseAcceleration;
                AccelerationCurve += stats.AccelerationCurve;
                Braking += stats.Braking;
                CoastingDrag += stats.CoastingDrag;
                Grip += stats.Grip;
                Steer += stats.Steer;
                AddedGravity += stats.AddedGravity;
                
                return;
            }

            Debug.LogWarning($"Cannot add {otherStats} to {this}");
        }

        public void ClampStats()
        {
            TopSpeed = Mathf.Max(TopSpeed, 0.001f);
            ReverseSpeed = Mathf.Max(TopSpeed, 0f);
            AccelerationCurve = Mathf.Clamp(AccelerationCurve, 0.2f, 1);
            Grip = Mathf.Clamp(Grip, 0.0f, 1.0f);
            Acceleration = Mathf.Clamp(Acceleration, 0, 1000);
        }
    }
}