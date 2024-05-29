using ScriptableArchitecture.Core;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class MovementStats : IDataPoint
    {
        [Min(0.001f), Tooltip("Top speed attainable when moving forward.")]
        public float TopSpeed;

        [Tooltip("How quickly the kart reaches top speed.")]
        public float Acceleration;

        [Min(0.001f), Tooltip("Top speed attainable when moving backward.")]
        public float ReverseSpeed;

        [Tooltip("How quickly the kart reaches top speed, when moving backward.")]
        public float ReverseAcceleration;

        [Tooltip("How quickly the kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
        [Range(0.2f, 1)]
        public float AccelerationCurve;

        [Tooltip("How quickly the kart slows down when the brake is applied.")]
        public float Braking;

        [Tooltip("How quickly the kart will reach a full stop when no inputs are made.")]
        public float CoastingDrag;

        [Range(0.0f, 1.0f)]
        [Tooltip("The amount of side-to-side friction.")]
        public float Grip;

        [Tooltip("How tightly the kart can turn left or right.")]
        public float Steer;

        [Tooltip("Additional gravity for when the kart is in the air.")]
        public float AddedGravity;

        public static MovementStats operator +(MovementStats stats1, MovementStats stats2)
        {
            return new MovementStats
            {
                Acceleration = stats1.Acceleration + stats2.Acceleration,
                AccelerationCurve = stats1.AccelerationCurve + stats2.AccelerationCurve,
                Braking = stats1.Braking + stats2.Braking,
                CoastingDrag = stats1.CoastingDrag + stats2.CoastingDrag,
                AddedGravity = stats1.AddedGravity + stats2.AddedGravity,
                Grip = stats1.Grip + stats2.Grip,
                ReverseAcceleration = stats1.ReverseAcceleration + stats2.ReverseAcceleration,
                ReverseSpeed = stats1.ReverseSpeed + stats2.ReverseSpeed,
                TopSpeed = stats1.TopSpeed + stats2.TopSpeed,
                Steer = stats1.Steer + stats2.Steer,
            };
        }
    }
}