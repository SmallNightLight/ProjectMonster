using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class DriftStats : IDataPoint, IStats
    {
        [Range(0.01f, 1f), Tooltip("The grip value when drifting")]
        public float DriftGrip = 0.8f;
        [Range(0f, 10f), Tooltip("Additional steer when the kart is drifting")]
        public float DriftAdditionalSteer = 3f;
        [Range(1f, 30f), Tooltip("The higher the angle, the easier it is to regain full grip")]
        public float MinAngleToFinishDrift = 10;
        [Range(0.01f, 0.99f), Tooltip("Mininum speed percentage to switch back to full grip")]
        public float MinSpeedPercentToFinishDrift = 0.8f;
        [Range(1f, 20f), Tooltip("The higher the value, the easier it is to control the drift steering")]
        public float DriftControl = 10f;
        [Range(0f, 20f), Tooltip("The lower the value, the longer the drift will last without trying to control it by steering")]
        public float DriftDampening = 10f;


        public void SetStats(IStats newStats)
        {
            if (newStats is DriftStats)
            {
                DriftStats stats = (DriftStats)newStats;

                DriftGrip = stats.DriftGrip;
                DriftAdditionalSteer = stats.DriftAdditionalSteer;
                MinAngleToFinishDrift = stats.MinAngleToFinishDrift;
                MinSpeedPercentToFinishDrift = stats.MinSpeedPercentToFinishDrift;
                DriftControl = stats.DriftControl;
                DriftDampening = stats.DriftDampening;

                return;
            }

            Debug.LogWarning($"Cannot set {this} to {newStats}");
        }

        public void AddStats(IStats otherStats)
        {
            if (otherStats is DriftStats)
            {
                DriftStats stats = (DriftStats)otherStats;

                DriftGrip += stats.DriftGrip;
                DriftAdditionalSteer += stats.DriftAdditionalSteer;
                MinAngleToFinishDrift += stats.MinAngleToFinishDrift;
                MinSpeedPercentToFinishDrift += stats.MinSpeedPercentToFinishDrift;
                DriftControl += stats.DriftControl;
                DriftDampening += stats.DriftDampening;

                return;
            }

            Debug.LogWarning($"Cannot add {otherStats} to {this}");
        }

        public void ClampStats()
        {
            DriftGrip = Mathf.Clamp(DriftGrip, 0.01f, 1f);
            DriftAdditionalSteer = Mathf.Clamp(DriftAdditionalSteer, 0f, 10f);
            MinAngleToFinishDrift = Mathf.Clamp(MinAngleToFinishDrift, 1f, 30f);
            MinSpeedPercentToFinishDrift = Mathf.Clamp(MinSpeedPercentToFinishDrift, 0.01f, 0.99f);
            DriftControl = Mathf.Clamp(DriftControl, 1f, 20f);
            DriftDampening = Mathf.Clamp(DriftDampening, 0f, 20f);
        }
    }
}