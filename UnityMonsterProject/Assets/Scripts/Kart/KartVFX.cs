using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static VisualWheel;

[RequireComponent(typeof(KartBase), typeof(KartMovement))]
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

    [SerializeField] private FloatReference _animationSteerSpeed;

    [Header("VFX")]
    [Tooltip("VFX that will be placed on the wheels when drifting.")]
    [SerializeField] private GameObject _driftSparkVFX;
    [Range(0.0f, 0.2f), Tooltip("Offset to displace the VFX to the side.")]
    [SerializeField] private float _driftSparkHorizontalOffset = 0.1f;
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

    [SerializeField] private Animation _hitAnimation;

    private Animator _characterAnimator;

    private List<(GameObject trailRoot, WheelCollider wheel, TrailRenderer trail)> m_DriftTrailInstances = new List<(GameObject, WheelCollider, TrailRenderer)>();
    private List<(WheelCollider wheel, float horizontalOffset, float rotation, VisualEffect sparks)> m_DriftSparkInstances = new List<(WheelCollider, float, float, VisualEffect)>();

    private KartBase _base;
    private KartMovement _kartMovement;

    private float _currentSteering;
    private float _currentAnimationSteering;

    private void Start()
    {
        _base = GetComponent<KartBase>();
        _kartMovement = GetComponent<KartMovement>();
        _characterAnimator = _base.CharacterVisualParent?.GetComponentInChildren<Animator>();

        InitializeComponents();
        InitializeParticleEffects();
    }

    [ContextMenu("Update components")]
    public void InitializeComponents()
    {
        foreach(var visualWheel in GetComponentsInChildren<VisualWheel>())
        {
            switch(visualWheel.WheelSide)
            {
                case VisualWheelSide.FrontLeft:
                    _wheelVisualFrontLeft = visualWheel.transform; 
                    break;
                case VisualWheelSide.FrontRight:
                    _wheelVisualFrontRight = visualWheel.transform;
                    break;
                case VisualWheelSide.RearLeft:
                    _wheelVisualRearLeft = visualWheel.transform;
                    break;
                case VisualWheelSide.RearRight:
                    _wheelVisualRearRight = visualWheel.transform;
                    break;
            }
        }
    }

    private void InitializeParticleEffects()
    {
        if (_driftSparkVFX != null)
        {
            AddSparkToWheel(_wheelColliderRearLeft, -_driftSparkHorizontalOffset, 0);
            AddSparkToWheel(_wheelColliderRearRight, _driftSparkHorizontalOffset, 180);
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
        GameObject vfx = Instantiate(_driftSparkVFX, wheel.transform, false);
        VisualEffect spark = vfx.GetComponent<VisualEffect>();
        spark.Stop();
        m_DriftSparkInstances.Add((wheel, horizontalOffset, -rotation, spark));
    }

    private void Update()
    {
        UpdateWheels();
        UpdateDriftVFXOrientation();
        UpdateAnimations();
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

    public void UpdateAnimations()
    {
        float targetSteering = _base.Input.SteerInput * _steeringLimit.Value;
        _currentAnimationSteering = Mathf.Lerp(_currentAnimationSteering, targetSteering, Time.deltaTime * _animationSteerSpeed.Value);

        if (_characterAnimator)
            _characterAnimator.SetFloat("Steer", _currentAnimationSteering / _steeringLimit.Value);
    }

    public void Hop()
    {
        if (_characterAnimator)
            _characterAnimator.SetTrigger("Hop");
    }

    public void Ability()
    {
        if (_characterAnimator)
            _characterAnimator.SetTrigger("Ability");
    }

    public void ChangeDriftState(bool active)
    {
        foreach (var vfx in m_DriftSparkInstances)
        {
            if (active && vfx.wheel.GetGroundHit(out WheelHit hit))
            {
                vfx.sparks.Play();
            }
            else
            {
                vfx.sparks.Stop();
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
            vfx.sparks.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, vfx.rotation, 0.0f);
        }

        foreach (var trail in m_DriftTrailInstances)
        {
            trail.trailRoot.transform.position = trail.wheel.transform.position - (trail.wheel.radius * Vector3.up) + (_driftTrailVerticalOffset * Vector3.up);
            trail.trailRoot.transform.rotation = transform.rotation;
        }
    }

    public void Jump(Vector3 position)
    {
        if (_characterAnimator)
            _characterAnimator.SetTrigger("Jump");

        if (_jumpVFX == null) return;
        
        Instantiate(_jumpVFX, position, Quaternion.identity);
    }

    public void Hit()
    {
        if (_hitAnimation != null)
            _hitAnimation.Play();

        if (_characterAnimator)
            _characterAnimator.SetTrigger("Hit");
    }

    //Overlay Boom effect

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if ((_obstacleLayers & (1 << collision.gameObject.layer)) != 0)
    //    {
    //        //Hit obstacle
    //        ContactPoint impactPoint = collision.GetContact(0);

    //        GameObject g = Instantiate(_effectBoom, impactPoint.point, Quaternion.identity);
    //        Destroy(g, 2);
    //    }
    //}
}