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

    [SerializeField] private FloatReference _steeringLimit;
    [SerializeField] private FloatReference _steeringSpeed = default(FloatReference);

    private KartBase _base;

    private float _currentSteering;

    private void Start()
    {
        _base = GetComponent<KartBase>();

        //_steeringLimit.InitializeInstance();
        //_steeringSpeed.InitializeInstance();
    }

    private void Update()
    {
        UpdateWheels();
    }

    private void UpdateWheels()
    {
        float targetSteering = _base.Input.SteerInput * _steeringLimit.Value;
        _currentSteering = Mathf.Lerp(_currentSteering, targetSteering, Time.deltaTime * _steeringSpeed.Value);

        _wheelColliderFrontLeft.GetWorldPose(out Vector3 pos, out Quaternion speedFrontLeft);
        _wheelColliderFrontLeft.GetWorldPose(out pos, out Quaternion speedFrontRight);
        _wheelColliderFrontLeft.GetWorldPose(out pos, out Quaternion speedRearLeft);
        _wheelColliderFrontLeft.GetWorldPose(out pos, out Quaternion speedRearRight);

        _wheelVisualFrontLeft.localRotation = Quaternion.Euler(new Vector3(speedFrontLeft.eulerAngles.x, _currentSteering, 0));
        _wheelVisualFrontRight.localRotation = Quaternion.Euler(new Vector3(speedFrontRight.eulerAngles.x, _currentSteering, 0));
        _wheelVisualRearRight.localRotation = Quaternion.Euler(new Vector3(speedRearLeft.eulerAngles.x, 0, 0));
        _wheelVisualRearLeft.localRotation = Quaternion.Euler(new Vector3(speedRearRight.eulerAngles.x, 0, 0));
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