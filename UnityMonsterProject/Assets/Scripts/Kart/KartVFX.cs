using ScriptableArchitecture.Data;
using UnityEngine;

[RequireComponent(typeof(KartBase))]
public class KartVFX : MonoBehaviour
{
    [SerializeField] private GameObject _effectBoom;
    [SerializeField] private LayerMask _obstacleLayers;

    [Header("Wheels Rotation")]
    [SerializeField] private WheelCollider _wheelColliderFrontLeft;
    [SerializeField] private WheelCollider _wheelColliderFrontRight;
    [SerializeField] private WheelCollider _wheelColliderRearLeft;
    [SerializeField] private WheelCollider _wheelColliderRearRight;

    [SerializeField] private Transform _wheelVisualFrontLeft;
    [SerializeField] private Transform _wheelVisualFrontRight;
    [SerializeField] private Transform _wheelVisualRearLeft;
    [SerializeField] private Transform _wheelVisualRearRight;

    [SerializeField] private float _wheelSpeedMultiplier = 10;
    [SerializeField] private float _steeringLimit = 30f;

    private KartBase _base;

    private float _currentSteering;

    private void Start()
    {
        _base = GetComponent<KartBase>();
    }

    private void Update()
    {
        UpdateWheels();
    }

    private void UpdateWheels()
    {
        //Set Speed
        //_wheelVisualFrontLeft.Rotate(Mathf.Clamp(_wheelColliderFrontLeft.rpm / 60 * 360, -_maxWheelRotationSpeed, _maxWheelRotationSpeed) * Time.deltaTime, 0, 0);
        //_wheelVisualFrontRight.Rotate(Mathf.Clamp(_wheelColliderFrontRight.rpm / 60 * 360, -_maxWheelRotationSpeed, _maxWheelRotationSpeed) * Time.deltaTime, 0, 0);
        //_wheelVisualRearLeft.Rotate(Mathf.Clamp(_wheelColliderRearLeft.rpm / 60 * 360, -_maxWheelRotationSpeed, _maxWheelRotationSpeed) * Time.deltaTime, 0, 0);
        //_wheelVisualRearRight.Rotate(Mathf.Clamp(_wheelColliderRearRight.rpm / 60 * 360, -_maxWheelRotationSpeed, _maxWheelRotationSpeed) * Time.deltaTime, 0, 0);

        // Rotate the wheels based on their RPM for spinning
        _wheelVisualFrontLeft.Rotate(_wheelColliderFrontLeft.rpm / 60 * 360 * _wheelSpeedMultiplier * Time.deltaTime, 0, 0);
        _wheelVisualFrontRight.Rotate(_wheelColliderFrontRight.rpm / 60 * 360 * _wheelSpeedMultiplier * Time.deltaTime, 0, 0);
        _wheelVisualRearLeft.Rotate(_wheelColliderRearLeft.rpm / 60 * 360 * _wheelSpeedMultiplier * Time.deltaTime, 0, 0);
        _wheelVisualRearRight.Rotate(_wheelColliderRearRight.rpm / 60 * 360 * _wheelSpeedMultiplier * Time.deltaTime, 0, 0);

        float steeringInput = _base.Input.SteerInput;
        float targetSteering = steeringInput * _steeringLimit;

        // Calculate the target steering rotation in local space
        Quaternion targetSteeringRotation = Quaternion.Euler(0, targetSteering, 0);

        // Smoothly interpolate the current steering rotation towards the target steering rotation
        _wheelVisualFrontLeft.localRotation = Quaternion.Lerp(_wheelVisualFrontLeft.localRotation, targetSteeringRotation, Time.deltaTime * 4f);
        _wheelVisualFrontRight.localRotation = Quaternion.Lerp(_wheelVisualFrontRight.localRotation, targetSteeringRotation, Time.deltaTime * 4f);

        // Apply the spinning rotation on top of the steering rotation
        _wheelVisualFrontLeft.localRotation *= Quaternion.Euler(_wheelColliderFrontLeft.rpm / 60 * 360 * _wheelSpeedMultiplier * Time.deltaTime, 0, 0);
        _wheelVisualFrontRight.localRotation *= Quaternion.Euler(_wheelColliderFrontRight.rpm / 60 * 360 * _wheelSpeedMultiplier * Time.deltaTime, 0, 0);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((_obstacleLayers & (1 << collision.gameObject.layer)) != 0)
        {
            //Hit obstacle
            ContactPoint impactPoint = collision.GetContact(0);

            GameObject g = Instantiate(_effectBoom, impactPoint.point, Quaternion.identity);
            Destroy(g, 2);
        }
    }
}