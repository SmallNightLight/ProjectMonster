using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float rotationSpeed = 10f;

    private Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned");
            return;
        }

        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            float angle = rotationSpeed * Time.deltaTime;
            offset = Quaternion.AngleAxis(angle, Vector3.up) * offset;
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}