using System;
using UnityEngine;
using System.Collections.Generic;
using ScriptableArchitecture.Data;
using UnityEngine.Events;

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

    private KartBase _base;
    private Rigidbody _rigidbody;

    private const float k_NullInput = 0.01f;
    private const float k_NullSpeed = 0.01f;
    private Vector3 m_VerticalReference = Vector3.up;

    //Drifting
    private bool _wantsToDrift = false;
    private bool IsDrifting = false;
    private float m_CurrentGrip = 1.0f;
    private float m_DriftTurningPower = 0.0f;
    private float m_PreviousGroundPercent = 1.0f;

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

        _wantsToDrift = _base.Input.IsAccelerating && _base.Input.IsBraking;// && Vector3.Dot(_rigidbody.velocity, transform.forward) > 0.0f;

        // apply our physics properties
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
            MoveVehicle(_base.Input.IsAccelerating, _base.Input.IsBraking, _base.Input.SteerInput);
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
            _rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime * _finalMovementStats.AddedGravity;
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
        newVelocity.y = _rigidbody.velocity.y;

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

            // If the karts lands with a forward not in the velocity direction, we start the drift
            //if (_groundPercent >= 0.0f && m_PreviousGroundPercent < 0.1f)
            //{
            //    Vector3 flattenVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, m_VerticalReference).normalized;
            //    if (Vector3.Dot(flattenVelocity, transform.forward * Mathf.Sign(accelInput)) < Mathf.Cos(_driftStats.MinAngleToFinishDrift * Mathf.Deg2Rad))
            //    {
            //        IsDrifting = true;
            //        m_CurrentGrip = _driftStats.DriftGrip;
            //        m_DriftTurningPower = 0.0f;
            //    }
            //}

            // Drift Management
            if (!IsDrifting)
            {
                if (_wantsToDrift && currentSpeed > maxSpeed * _driftStats.Value.MinSpeedPercentToFinishDrift)
                {
                    IsDrifting = true;
                    m_DriftTurningPower = turningPower + (Mathf.Sign(turningPower) * _driftStats.Value.DriftAdditionalSteer);
                    m_CurrentGrip = _driftStats.Value.DriftGrip;

                    _changeDriftState.Invoke(true);
                }
            }

            if (IsDrifting)
            {
                float turnInputAbs = Mathf.Abs(turnInput);
                if (turnInputAbs < k_NullInput)
                    m_DriftTurningPower = Mathf.MoveTowards(m_DriftTurningPower, 0.0f, Mathf.Clamp01(_driftStats.Value.DriftDampening * Time.fixedDeltaTime));

                // Update the turning power based on input
                float driftMaxSteerValue = _finalMovementStats.Steer + _driftStats.Value.DriftAdditionalSteer;
                m_DriftTurningPower = Mathf.Clamp(m_DriftTurningPower + (turnInput * Mathf.Clamp01(_driftStats.Value.DriftControl * Time.fixedDeltaTime)), -driftMaxSteerValue, driftMaxSteerValue);

                bool facingVelocity = Vector3.Dot(_rigidbody.velocity.normalized, transform.forward * Mathf.Sign(accelInput)) > Mathf.Cos(_driftStats.Value.MinAngleToFinishDrift * Mathf.Deg2Rad);

                bool canEndDrift = true;
                if (isBraking)
                    canEndDrift = false;
                else if (!facingVelocity)
                    canEndDrift = false;
                else if (turnInputAbs >= k_NullInput && currentSpeed > maxSpeed * _driftStats.Value.MinSpeedPercentToFinishDrift)
                    canEndDrift = false;

                if (canEndDrift || currentSpeed < k_NullSpeed)
                {
                    // No Input, and car aligned with speed direction => Stop the drift
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hit")
        {
            Debug.Log("Hit");
        }
    }
}