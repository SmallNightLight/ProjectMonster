using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ScriptableArchitecture.Data;
using UnityEngine.Events;
using Unity.Mathematics;

[RequireComponent(typeof(KartBase), typeof(Rigidbody))]
public class KartMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private MovementStatsReference _movementStats;
    [SerializeField] private DriftStatsReference _driftStats;
    [SerializeField] private SuspensionStatsReference _suspensionStats;
    [SerializeField] private PhysicStatsReference _physicsStats;

    [Header("Components")]
    [SerializeField] private Transform _centerOfMass;

    [SerializeField] private WheelCollider _wheelColliderFrontLeft;
    [SerializeField] private WheelCollider _wheelColliderFrontRight;
    [SerializeField] private WheelCollider _wheelColliderRearLeft;
    [SerializeField] private WheelCollider _wheelColliderRearRight;

    [SerializeField] private Transform _smartRayRight;
    [SerializeField] private Transform _smartRayLeft;

    [SerializeField] private float _smartRayDistance;
    [SerializeField] private LayerMask _smartSteeringLayers;
    [Range(0, 10), SerializeField] private float _smartSteeringAmount = 6f;
    [SerializeField] private float _distancePower = 1.5f;

    private KartBase _base;
    private Rigidbody _rigidbody;

    private const float k_NullInput = 0.01f;
    private const float k_NullSpeed = 0.01f;
    private Vector3 m_VerticalReference = Vector3.up;

    //Drifting
    [SerializeField] private float _minSteerForDrift = 0.1f;
    [SerializeField] private float _minDriftSteer  = 0.1f;
    [SerializeField] private float _driftTurnPower = 10f;
    [SerializeField] private AnimationCurve _driftCurve;

    [SerializeField] private Vector2 _curveValues;
    [SerializeField] private float _curveSpeed;

    private bool _wantsToDrift = false;
    private bool IsDrifting = false;
    private float m_CurrentGrip = 1.0f;
    private int _driftDirection = 0;
    private float m_DriftTurningPower = 0.0f;
    private float m_PreviousGroundPercent = 1.0f;
    private float _curveValue = 0f;

    //Hop
    private bool _wantsToHop = false;
    bool _doHop = false;
    [SerializeField] private float _hopPower;
    [SerializeField] private float _hopDuration;
    private bool _isHopping;
    [SerializeField] private float _hopDownAir;

    private List<AbilityData> _activeAbilities = new List<AbilityData>();
    private bool m_CanMove = true;

    //Final stats
    private MovementStats _finalMovementStats;
    private DriftStats _finalDriftStats;
    private SuspensionStats _finalSuspensionStats;
    private PhysicStats _finalPhysicsStats;

    private Quaternion m_LastValidRotation;
    private Vector3 m_LastCollisionNormal;
    private bool m_HasCollision;
    private bool m_InAir = false;
    private float _airPercent;
    private float _groundPercent;

    //Events
    [SerializeField] private UnityEvent<Vector3> _jumpEvent;
    [SerializeField] private UnityEvent<bool> _changeDriftState;

    public void SetCanMove(bool move) => m_CanMove = move;
    public float GetMaxSpeed() => Mathf.Max(_finalMovementStats.TopSpeed, _finalMovementStats.ReverseSpeed);

    void UpdateSuspensionParams(WheelCollider wheel)
    {
        wheel.suspensionDistance = _suspensionStats.Value.SuspensionHeight;
        wheel.center = new Vector3(0.0f, _suspensionStats.Value.WheelsPositionVerticalOffset, 0.0f);
        JointSpring spring = wheel.suspensionSpring;
        spring.spring = _suspensionStats.Value.SuspensionSpring;
        spring.damper = _suspensionStats.Value.SuspensionDamp;
        wheel.suspensionSpring = spring;
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _base = GetComponent<KartBase>();

        InitializeStats();

        UpdateSuspensionParams(_wheelColliderFrontLeft);
        UpdateSuspensionParams(_wheelColliderFrontRight);
        UpdateSuspensionParams(_wheelColliderRearLeft);
        UpdateSuspensionParams(_wheelColliderRearRight);

        m_CurrentGrip = _movementStats.Value.Grip;
    }

    private void InitializeStats()
    {
        _finalMovementStats = new MovementStats();
        _finalDriftStats = new DriftStats();
        _finalSuspensionStats = new SuspensionStats();
        _finalPhysicsStats = new PhysicStats();

        UpdateAllStats();
    }

    void FixedUpdate()
    {
        UpdateAllStats();

        UpdateSuspensionParams(_wheelColliderFrontLeft);
        UpdateSuspensionParams(_wheelColliderFrontRight);
        UpdateSuspensionParams(_wheelColliderRearLeft);
        UpdateSuspensionParams(_wheelColliderRearRight);

        _wantsToHop = _base.Input.IsAccelerating && _base.Input.IsBraking && !IsDrifting;
        _wantsToDrift = _wantsToHop && Mathf.Abs(_base.Input.SteerInput) > _minSteerForDrift;

        _rigidbody.centerOfMass = transform.InverseTransformPoint(_centerOfMass.position);

        int groundedCount = 0;
        if (_wheelColliderFrontLeft.isGrounded && _wheelColliderFrontLeft.GetGroundHit(out WheelHit hit))
            groundedCount++;
        if (_wheelColliderFrontRight.isGrounded && _wheelColliderFrontRight.GetGroundHit(out hit))
            groundedCount++;
        if (_wheelColliderRearLeft.isGrounded && _wheelColliderRearLeft.GetGroundHit(out hit))
            groundedCount++;
        if (_wheelColliderRearRight.isGrounded && _wheelColliderRearRight.GetGroundHit(out hit))
            groundedCount++;

        //Calculate how grounded and airborne we are
        _groundPercent = (float)groundedCount / 4.0f;
        _airPercent = 1 - _groundPercent;

        //Apply vehicle physics
        if (m_CanMove)
        {
            MoveVehicle(_base.Input.IsAccelerating, _base.Input.IsBraking && !_base.Input.IsAccelerating, _base.Input.SteerInput);
        }
        GroundAirbourne();

        m_PreviousGroundPercent = _groundPercent;
    }

    private void UpdateAllStats()
    {
        UpdateStats(_movementStats.Value, _finalMovementStats);
        UpdateStats(_driftStats.Value, _finalDriftStats);
        UpdateStats(_suspensionStats.Value, _finalSuspensionStats);
        UpdateStats(_physicsStats.Value, _finalPhysicsStats);
    }

    private void UpdateStats(IStats baseStats, IStats finalStats)
    {
        finalStats.SetStats(baseStats);

        foreach (AbilityData ability in _activeAbilities)
        {
            foreach (var variable in ability.AdditionalStats)
            {
                IStats additionalStats = variable.GetStats();

                if (finalStats.GetType() != additionalStats.GetType()) continue;

                finalStats.AddStats(additionalStats);
            }
        }

        finalStats.ClampStats();
    }

    public void AddAbility(AbilityData ability) => _activeAbilities.Add(ability);

    public void RemoveAbility(AbilityData ability) => _activeAbilities.Remove(ability);

    void GroundAirbourne()
    {
        // while in the air, fall faster
        if (_airPercent >= 1)
        {
            _rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime * (_isHopping ? _hopDownAir : _finalMovementStats.AddedGravity);
        }
    }

    public void Reset()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x = euler.z = 0f;
        transform.rotation = Quaternion.Euler(euler);
    }

    public float LocalSpeed()
    {
        if (m_CanMove)
        {
            float dot = Vector3.Dot(transform.forward, _rigidbody.velocity);
            if (Mathf.Abs(dot) > 0.1f)
            {
                float speed = _rigidbody.velocity.magnitude;
                return dot < 0 ? -(speed / _finalMovementStats.ReverseSpeed) : (speed / _finalMovementStats.TopSpeed);
            }
            return 0f;
        }
        else
        {
            // use this value to play kart sound when it is waiting the race start countdown.
            return _base.Input.IsAccelerating ? 1 : 0;
        }
    }

    void OnCollisionEnter(Collision collision) => m_HasCollision = true;
    void OnCollisionExit(Collision collision) => m_HasCollision = false;

    void OnCollisionStay(Collision collision)
    {
        m_HasCollision = true;
        m_LastCollisionNormal = Vector3.zero;
        float dot = -1.0f;

        foreach (var contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > dot)
                m_LastCollisionNormal = contact.normal;
        }
    }

    void MoveVehicle(bool accelerate, bool brake, float turnInput)
    {
        float smartSteering = SmartSteering();
        smartSteering *= 2;
        smartSteering *= _smartSteeringAmount;
        turnInput = Mathf.Clamp(turnInput + smartSteering, -1f, 1f);

        float accelInput = (accelerate ? 1.0f : 0.0f) - (brake ? 1.0f : 0.0f);

        // manual acceleration curve coefficient scalar
        float accelerationCurveCoeff = 5;
        Vector3 localVel = transform.InverseTransformVector(_rigidbody.velocity);

        bool accelDirectionIsFwd = accelInput >= 0;
        bool localVelDirectionIsFwd = localVel.z >= 0;

        // use the max speed for the direction we are going--forward or reverse.
        float maxSpeed = localVelDirectionIsFwd ? _finalMovementStats.TopSpeed : _finalMovementStats.ReverseSpeed;
        float accelPower = accelDirectionIsFwd ? _finalMovementStats.Acceleration : _finalMovementStats.ReverseAcceleration;

        float currentSpeed = _rigidbody.velocity.magnitude;
        float accelRampT = currentSpeed / maxSpeed;
        float multipliedAccelerationCurve = _finalMovementStats.AccelerationCurve * accelerationCurveCoeff;
        float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

        bool isBraking = (localVelDirectionIsFwd && brake) || (!localVelDirectionIsFwd && accelerate);

        // if we are braking (moving reverse to where we are going)
        // use the braking accleration instead
        float finalAccelPower = isBraking ? _finalMovementStats.Braking : accelPower;

        float finalAcceleration = finalAccelPower * accelRamp;

        // apply inputs to forward/backward
        float turningPower = IsDrifting ? m_DriftTurningPower : turnInput * _finalMovementStats.Steer;

        Quaternion turnAngle = Quaternion.AngleAxis(turningPower, transform.up);
        Vector3 fwd = turnAngle * transform.forward;
        Vector3 movement = fwd * accelInput * finalAcceleration * ((m_HasCollision || _groundPercent > 0.0f) ? 1.0f : 0.0f);

        // forward movement
        bool wasOverMaxSpeed = currentSpeed >= maxSpeed;
        
        // if over max speed, cannot accelerate faster.
        if (wasOverMaxSpeed && !isBraking)
            movement *= 0.0f;

        Vector3 newVelocity = _rigidbody.velocity + movement * Time.fixedDeltaTime;
        newVelocity.y = _rigidbody.velocity.y + (_doHop ? _hopPower : 0f);

        if (_doHop) _doHop = false;

        //  clamp max speed if we are on ground
        if (_groundPercent > 0.0f && !wasOverMaxSpeed)
        {
            newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
        }

        // coasting is when we aren't touching accelerate
        if (Mathf.Abs(accelInput) < k_NullInput && _groundPercent > 0.0f)
        {
            newVelocity = Vector3.MoveTowards(newVelocity, new Vector3(0, _rigidbody.velocity.y, 0), Time.fixedDeltaTime * _finalMovementStats.CoastingDrag);
        }

        _rigidbody.velocity = newVelocity;

        // Drift
        if (_groundPercent > 0.0f)
        {
            if (m_InAir)
            {
                m_InAir = false;
                _jumpEvent.Invoke(transform.position);
            }

            // manual angular velocity coefficient
            float angularVelocitySteering = 0.4f;
            float angularVelocitySmoothSpeed = 20f;

            // turning is reversed if we're going in reverse and pressing reverse
            if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
                angularVelocitySteering *= -1.0f;

            var angularVel = _rigidbody.angularVelocity;

            // move the Y angular velocity towards our target
            angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.fixedDeltaTime * angularVelocitySmoothSpeed);

            // apply the angular velocity
            _rigidbody.angularVelocity = angularVel;

            // rotate rigidbody's velocity as well to generate immediate velocity redirection
            // manual velocity steering coefficient
            float velocitySteering = 25f;

            //Drift Management
            if (!IsDrifting && _wantsToHop)
                StartCoroutine(Hop());

            if (IsDrifting)
            {
                if (_curveValue < _curveValues.y)
                    _curveValue += Time.fixedDeltaTime * _curveSpeed;

                float driftTurn;

                if (_driftDirection == 1)
                {
                    driftTurn = math.remap(-1, 1, 0, 1, turnInput);
                    driftTurn = _curveValue * (Mathf.Pow(driftTurn - _minDriftSteer, 2));
                }
                else
                {
                    driftTurn = math.remap(-1, 1, -1, 0, turnInput);
                    driftTurn *= -1;
                    driftTurn = _curveValue * (Mathf.Pow(driftTurn - _minDriftSteer, 2));
                    driftTurn *= -1;
                }

                m_DriftTurningPower = driftTurn * _driftTurnPower;

                if (!_base.Input.IsBraking)
                {
                    IsDrifting = false;
                    m_CurrentGrip = _finalMovementStats.Grip;
                }
            }

            // rotate our velocity based on current steer value
            _rigidbody.velocity = Quaternion.AngleAxis(turningPower * Mathf.Sign(localVel.z) * velocitySteering * m_CurrentGrip * Time.fixedDeltaTime, transform.up) * _rigidbody.velocity;
        }
        else
        {
            m_InAir = true;
        }

        bool validPosition = false;
        if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out RaycastHit hit, 3.0f, 1 << 9 | 1 << 10 | 1 << 11)) // Layer: ground (9) / Environment(10) / Track (11)
        {
            Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > hit.normal.y) ? m_LastCollisionNormal : hit.normal;
            m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(_physicsStats.Value.AirborneReorientationCoefficient * Time.fixedDeltaTime * (_groundPercent > 0.0f ? 10.0f : 1.0f)));    // Blend faster if on ground
        }
        else
        {
            Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > 0.0f) ? m_LastCollisionNormal : Vector3.up;
            m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(_physicsStats.Value.AirborneReorientationCoefficient * Time.fixedDeltaTime));
        }

        validPosition = _groundPercent > 0.7f && !m_HasCollision && Vector3.Dot(m_VerticalReference, Vector3.up) > 0.9f;

        // Airborne / Half on ground management
        if (_groundPercent < 0.7f)
        {
            _rigidbody.angularVelocity = new Vector3(0.0f, _rigidbody.angularVelocity.y * 0.98f, 0.0f);
            Vector3 finalOrientationDirection = Vector3.ProjectOnPlane(transform.forward, m_VerticalReference);
            finalOrientationDirection.Normalize();
            if (finalOrientationDirection.sqrMagnitude > 0.0f)
            {
                _rigidbody.MoveRotation(Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(finalOrientationDirection, m_VerticalReference), Mathf.Clamp01(_physicsStats.Value.AirborneReorientationCoefficient * Time.fixedDeltaTime)));
            }
        }
        else if (validPosition)
        {
            m_LastValidRotation.eulerAngles = new Vector3(0.0f, transform.rotation.y, 0.0f);
        }

        _changeDriftState.Invoke(IsDrifting && _groundPercent > 0.0f);
    }

    private float SmartSteering()
    {
        if (_smartRayLeft == null || _smartRayRight == null) return 0f;

        float speedDistance = _smartRayDistance * (Mathf.Pow(LocalSpeed(), _distancePower));

        bool right = Physics.Raycast(_smartRayLeft.position, _smartRayLeft.forward, out var hitRight, speedDistance, _smartSteeringLayers);
        bool left = Physics.Raycast(_smartRayRight.position, _smartRayRight.forward, out var hitLeft, speedDistance, _smartSteeringLayers);

        Debug.DrawRay(_smartRayLeft.position, _smartRayLeft.forward * speedDistance, left ? Color.yellow : Color.green);
        Debug.DrawRay(_smartRayRight.position, _smartRayRight.forward * speedDistance, right ? Color.yellow : Color.green);

        if (!right && !left) return 0f;

        float middleDistance = (hitRight.distance + hitLeft.distance) / 2f;
        float distancePercentage = middleDistance / speedDistance;

        if (right && left)
        {
            if (hitRight.distance > hitLeft.distance)
                return -(hitLeft.distance / hitRight.distance * distancePercentage);
            else
                return hitRight.distance / hitLeft.distance * distancePercentage;  
        }

        if (right)
        {
            return distancePercentage;
        }

        if (left)
        {
            return -distancePercentage;
        }

        return 0f;
    }

    private IEnumerator Hop()
    {
        _doHop = true;
        _isHopping = true;
        yield return new WaitForSeconds(_hopDuration);
        _isHopping = false;

        //Start drift if possible
        float turnInput = _base.Input.SteerInput;
        float turningPower = IsDrifting ? m_DriftTurningPower : turnInput * _finalMovementStats.Steer;

        if (Vector3.Dot(_rigidbody.velocity, transform.forward) > 0.0f && _rigidbody.velocity.magnitude > _finalMovementStats.TopSpeed * _driftStats.Value.MinSpeedPercentToFinishDrift && _wantsToDrift)
        {
            IsDrifting = true;
            m_DriftTurningPower = turningPower + (Mathf.Sign(turningPower) * _driftStats.Value.DriftAdditionalSteer);
            m_CurrentGrip = _driftStats.Value.DriftGrip;
            _driftDirection = turnInput > 0f ? 1 : -1;
            _curveValue = _curveValues.x;
            _changeDriftState.Invoke(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hit")
        {
            Debug.Log("Hit");
        }
    }
}

//Drift to do
//jump before drift
//Start drift after jump
//rotate car in drift direction