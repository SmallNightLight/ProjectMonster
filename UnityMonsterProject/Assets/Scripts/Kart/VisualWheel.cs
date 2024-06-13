using UnityEngine;

public class VisualWheel : MonoBehaviour
{
    public VisualWheelSide WheelSide;

    public enum VisualWheelSide{
        None, FrontLeft, FrontRight, RearLeft, RearRight
    }
}