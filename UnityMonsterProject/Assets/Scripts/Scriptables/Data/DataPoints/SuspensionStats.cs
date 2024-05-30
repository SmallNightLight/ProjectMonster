using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class SuspensionStats : IDataPoint, IStats
    {
        [Range(0f, 1f), Tooltip("The maximum extension possible between the kart's body and the wheels")]
        public float SuspensionHeight = 0.2f;
        [Range(10f, 100000f), Tooltip("The higher the value, the stiffer the suspension will be")]
        public float SuspensionSpring = 20000f;
        [Range(0f, 5000f), Tooltip("The higher the value, the faster the kart will stabilize itself")]
        public float SuspensionDamp = 500f;
        [Range(-1f, 1f), Tooltip("Vertical offset to adjust the position of the wheels relative to the kart's body")]
        public float WheelsPositionVerticalOffset = 0f;


        public void SetStats(IStats newStats)
        {
            if (newStats is SuspensionStats)
            {
                SuspensionStats stats = (SuspensionStats)newStats;

                SuspensionHeight = stats.SuspensionHeight;
                SuspensionSpring = stats.SuspensionSpring;
                SuspensionDamp = stats.SuspensionDamp;
                WheelsPositionVerticalOffset = stats.WheelsPositionVerticalOffset;

                return;
            }

            Debug.LogWarning($"Cannot set {this} to {newStats}");
        }

        public void AddStats(IStats otherStats)
        {
            if (otherStats is SuspensionStats)
            {
                SuspensionStats stats = (SuspensionStats)otherStats;

                SuspensionHeight += stats.SuspensionHeight;
                SuspensionSpring += stats.SuspensionSpring;
                SuspensionDamp += stats.SuspensionDamp;
                WheelsPositionVerticalOffset += stats.WheelsPositionVerticalOffset;

                return;
            }

            Debug.LogWarning($"Cannot add {otherStats} to {this}");
        }

        public void ClampStats()
        {
            SuspensionHeight = Mathf.Clamp(SuspensionHeight, 0f, 1f);
            SuspensionSpring = Mathf.Clamp(SuspensionSpring, 10f, 100000f);
            SuspensionDamp = Mathf.Clamp(SuspensionDamp, 0f, 5000f);
            WheelsPositionVerticalOffset = Mathf.Clamp(WheelsPositionVerticalOffset, -1f, 1f);
        }
    }
}