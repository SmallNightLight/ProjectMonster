using ScriptableArchitecture.Data;
using System.Collections.Generic;
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
    [SerializeField] private FloatReference _steeringSpeed;


    [Header("VFX")]
    [Tooltip("VFX that will be placed on the wheels when drifting.")]
    [SerializeField] private ParticleSystem _driftSparkVFX;
    [Range(0.0f, 0.2f), Tooltip("Offset to displace the VFX to the side.")]
    [SerializeField] private float _driftSparkHorizontalOffset = 0.1f;
    [Range(0.0f, 90.0f), Tooltip("Angle to rotate the VFX.")]
    [SerializeField] private float _driftSparkRotation = 17.0f;
    [Tooltip("VFX that will be placed on the wheels when drifting.")]
    [SerializeField] private GameObject _driftTrailPrefab;
    [Range(-0.1f, 0.1f), Tooltip("Vertical to move the trails up or down and ensure they are above the ground.")]
    [SerializeField] private float _driftTrailVerticalOffset;
    [Tooltip("VFX that will spawn upon landing, after a jump.")]
    [SerializeField] private GameObject _jumpVFX;
    [Tooltip("VFX that is spawn on the nozzles of the kart.")]
    [SerializeField] private GameObject _nozzleVFX;
    [Tooltip("List of the kart's nozzles.")]
    [SerializeField] private List<Transform> _nozzles;

    private List<(GameObject trailRoot, WheelCollider wheel, TrailRenderer trail)> m_DriftTrailInstances = new List<(GameObject, WheelCollider, TrailRenderer)>();
    private List<(WheelCollider wheel, float horizontalOffset, float rotation, ParticleSystem sparks)> m_DriftSparkInstances = new List<(WheelCollider, float, float, ParticleSystem)>();

    private KartBase _base;

    private float _currentSteering;


    private void Start()
    {
        _base = GetComponent<KartBase>();

        InitializeParticleEffects();
    }

    private void InitializeParticleEffects()
    {
        if (_driftSparkVFX != null)
        {
            AddSparkToWheel(_wheelColliderRearLeft, -_driftSparkHorizontalOffset, -_driftSparkRotation);
            AddSparkToWheel(_wheelColliderRearRight, _driftSparkHorizontalOffset, _driftSparkRotation);
        }

        if (_driftTrailPrefab != null)
        {
            AddTrailToWheel(_wheelColliderRearLeft);
            AddTrailToWheel(_wheelColliderRearRight);
        }

        if (_nozzleVFX != null)
        {
            foreach (var nozzle in _nozzles)
            {
                Instantiate(_nozzleVFX, nozzle, false);
            }
        }
    }

    void AddTrailToWheel(WheelCollider wheel)
    {
        GameObject trailRoot = Instantiate(_driftTrailPrefab, gameObject.transform, false);
        TrailRenderer trail = trailRoot.GetComponentInChildren<TrailRenderer>();
        trail.emitting = false;
        m_DriftTrailInstances.Add((trailRoot, wheel, trail));
    }

    void AddSparkToWheel(WheelCollider wheel, float horizontalOffset, float rotation)
    {
        GameObject vfx = Instantiate(_driftSparkVFX.gameObject, wheel.transform, false);
        ParticleSystem spark = vfx.GetComponent<ParticleSystem>();
        spark.Stop();
        m_DriftSparkInstances.Add((wheel, horizontalOffset, -rotation, spark));
    }

    private void Update()
    {
        UpdateWheels();
        UpdateDriftVFXOrientation();
    }

    private void UpdateWheels()
    {
        float targetSteering = _base.Input.SteerInput * _steeringLimit.Value;
        _currentSteering = Mathf.Lerp(_currentSteering, targetSteering, Time.deltaTime * _steeringSpeed.Value);

        _wheelColliderFrontLeft.GetWorldPose(out Vector3 pos, out Quaternion speedFrontLeft);
        _wheelColliderFrontLeft.GetWorldPose(out pos, out Quaternion speedFrontRight);
        _wheelColliderFrontLeft.GetWorldPose(out pos, out Quaternion speedRearLeft);
        _wheelColliderFrontLeft.GetWorldPose(out pos, out Quaternion speedRearRight);

        if (_wheelVisualFrontLeft)
            _wheelVisualFrontLeft.localRotation = Quaternion.Euler(speedFrontLeft.eulerAngles.x, _currentSteering, 0);

        if (_wheelVisualFrontRight)
            _wheelVisualFrontRight.localRotation = Quaternion.Euler(new Vector3(speedFrontRight.eulerAngles.x, _currentSteering, 0));

        if (_wheelVisualRearRight)
            _wheelVisualRearRight.localRotation = Quaternion.Euler(new Vector3(speedRearLeft.eulerAngles.x, 0, 0));

        if (_wheelVisualRearLeft)
            _wheelVisualRearLeft.localRotation = Quaternion.Euler(new Vector3(speedRearRight.eulerAngles.x, 0, 0));
    }

    public void ChangeDriftState(bool active)
    {
        foreach (var vfx in m_DriftSparkInstances)
        {
            if (active && vfx.wheel.GetGroundHit(out WheelHit hit))
            {
                if (!vfx.sparks.isPlaying)
                    vfx.sparks.Play();
            }
            else
            {
                if (vfx.sparks.isPlaying)
                    vfx.sparks.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        foreach (var trail in m_DriftTrailInstances)
            trail.Item3.emitting = active && trail.wheel.GetGroundHit(out WheelHit hit);
    }

    private void UpdateDriftVFXOrientation()
    {
        foreach (var vfx in m_DriftSparkInstances)
        {
            vfx.sparks.transform.position = vfx.wheel.transform.position - (vfx.wheel.radius * Vector3.up) + (_driftTrailVerticalOffset * Vector3.up) + (transform.right * vfx.horizontalOffset);
            vfx.sparks.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, vfx.rotation);
        }

        foreach (var trail in m_DriftTrailInstances)
        {
            trail.trailRoot.transform.position = trail.wheel.transform.position - (trail.wheel.radius * Vector3.up) + (_driftTrailVerticalOffset * Vector3.up);
            trail.trailRoot.transform.rotation = transform.rotation;
        }
    }

    public void Jump(Vector3 position)
    {
        if (_jumpVFX == null) return;
        
        Instantiate(_jumpVFX, position, Quaternion.identity);
    }


    //Overlay Boom effect

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