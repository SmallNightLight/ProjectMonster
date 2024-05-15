using System;
using UnityEngine;
using System.Collections.Generic;
using ScriptableArchitecture.Data;

namespace KartGame.KartSystems
{
    [RequireComponent(typeof(Rigidbody))]
    public class ArcadeKart : MonoBehaviour
    {
        [Serializable]
        public class StatPowerup
        {
            public Stats modifiers;
            public string PowerUpID;
            public float ElapsedTime;
            public float MaxTime;
        }

        [Serializable]
        public struct Stats
        {
            [Min(0.001f), Tooltip("Top speed attainable when moving forward.")]
            public float TopSpeed;

            [Tooltip("How quickly the kart reaches top speed.")]
            public float Acceleration;

            [Min(0.001f), Tooltip("Top speed attainable when moving backward.")]
            public float ReverseSpeed;

            [Tooltip("How quickly the kart reaches top speed, when moving backward.")]
            public float ReverseAcceleration;

            [Tooltip("How quickly the kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
            [Range(0.2f, 1)]
            public float AccelerationCurve;

            [Tooltip("How quickly the kart slows down when the brake is applied.")]
            public float Braking;

            [Tooltip("How quickly the kart will reach a full stop when no inputs are made.")]
            public float CoastingDrag;

            [Range(0.0f, 1.0f)]
            [Tooltip("The amount of side-to-side friction.")]
            public float Grip;

            [Tooltip("How tightly the kart can turn left or right.")]
            public float Steer;

            [Tooltip("Additional gravity for when the kart is in the air.")]
            public float AddedGravity;

            // allow for stat adding for powerups.
            public static Stats operator +(Stats a, Stats b)
            {
                return new Stats
                {
                    Acceleration        = a.Acceleration + b.Acceleration,
                    AccelerationCurve   = a.AccelerationCurve + b.AccelerationCurve,
                    Braking             = a.Braking + b.Braking,
                    CoastingDrag        = a.CoastingDrag + b.CoastingDrag,
                    AddedGravity        = a.AddedGravity + b.AddedGravity,
                    Grip                = a.Grip + b.Grip,
                    ReverseAcceleration = a.ReverseAcceleration + b.ReverseAcceleration,
                    ReverseSpeed        = a.ReverseSpeed + b.ReverseSpeed,
                    TopSpeed            = a.TopSpeed + b.TopSpeed,
                    Steer               = a.Steer + b.Steer,
                };
            }
        }

        [Serializable]
        public struct DrifStats
        {
            [Range(0.01f, 1.0f), Tooltip("The grip value when drifting.")]
            public float DriftGrip;
            [Range(0.0f, 10.0f), Tooltip("Additional steer when the kart is drifting.")]
            public float DriftAdditionalSteer;
            [Range(1.0f, 30.0f), Tooltip("The higher the angle, the easier it is to regain full grip.")]
            public float MinAngleToFinishDrift;
            [Range(0.01f, 0.99f), Tooltip("Mininum speed percentage to switch back to full grip.")]
            public float MinSpeedPercentToFinishDrift;
            [Range(1.0f, 20.0f), Tooltip("The higher the value, the easier it is to control the drift steering.")]
            public float DriftControl;
            [Range(0.0f, 20.0f), Tooltip("The lower the value, the longer the drift will last without trying to control it by steering.")]
            public float DriftDampening;
        }

        [SerializeField] private InputAssetReference _input;

        private float _airPercent;
        private float _groundPercent;

        public Stats _baseStats = new Stats
        {
            TopSpeed            = 10f,
            Acceleration        = 5f,
            AccelerationCurve   = 4f,
            Braking             = 10f,
            ReverseAcceleration = 5f,
            ReverseSpeed        = 5f,
            Steer               = 5f,
            CoastingDrag        = 4f,
            Grip                = .95f,
            AddedGravity        = 1f,
        };

        [Header("Components")]
        private Rigidbody _rigidbody;

        [Tooltip("The transform that determines the position of the kart's mass.")]
        [SerializeField] private Transform _centerOfMass;

        private List<GameObject> _visualWheels;

        [Header("Vehicle Physics")]
        [Range(0.0f, 20.0f), Tooltip("Coefficient used to reorient the kart in the air. The higher the number, the faster the kart will readjust itself along the horizontal plane.")]
        public float AirborneReorientationCoefficient = 3.0f;

        public DrifStats _driftStats = new DrifStats
        {
            DriftGrip = 0.4f,
            DriftAdditionalSteer = 5.0f,
            MinAngleToFinishDrift = 10.0f,
            MinSpeedPercentToFinishDrift = 0.5f,
            DriftControl = 10.0f,
            DriftDampening = 10.0f
        };

        [Header("VFX")]
        [SerializeField] private bool _enableParticles;
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

        [Header("Suspensions")]
        [Tooltip("The maximum extension possible between the kart's body and the wheels.")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float _suspensionHeight = 0.2f;
        [Range(10.0f, 100000.0f), Tooltip("The higher the value, the stiffer the suspension will be.")]
        [SerializeField] private float _suspensionSpring = 20000.0f;
        [Range(0.0f, 5000.0f), Tooltip("The higher the value, the faster the kart will stabilize itself.")]
        [SerializeField] private float _suspensionDamp = 500.0f;
        [Tooltip("Vertical offset to adjust the position of the wheels relative to the kart's body.")]
        [Range(-1.0f, 1.0f)]
        [SerializeField] private float _wheelsPositionVerticalOffset = 0.0f;

        [Header("Physical Wheels")]
        [Tooltip("The physical representations of the Kart's wheels.")]
        [SerializeField] private WheelCollider _wheelColliderFrontLeft;
        [SerializeField] private WheelCollider _wheelColliderFrontRight;
        [SerializeField] private WheelCollider _wheelColliderRearLeft;
        [SerializeField] private WheelCollider _wheelColliderRearRight;

        [Tooltip("Which layers the wheels will detect.")]
        [SerializeField] private LayerMask _groundLayers = Physics.DefaultRaycastLayers;

        private const float k_NullInput = 0.01f;
        private const float k_NullSpeed = 0.01f;
        private Vector3 m_VerticalReference = Vector3.up;

        //Drifting
        private bool _wantsToDrift = false;
        private bool IsDrifting = false;
        private float m_CurrentGrip = 1.0f;
        private float m_DriftTurningPower = 0.0f;
        private float m_PreviousGroundPercent = 1.0f;
        private readonly List<(GameObject trailRoot, WheelCollider wheel, TrailRenderer trail)> m_DriftTrailInstances = new List<(GameObject, WheelCollider, TrailRenderer)>();
        private readonly List<(WheelCollider wheel, float horizontalOffset, float rotation, ParticleSystem sparks)> m_DriftSparkInstances = new List<(WheelCollider, float, float, ParticleSystem)>();

        //Can the kart move?
        private bool m_CanMove = true;
        private List<StatPowerup> m_ActivePowerupList = new List<StatPowerup>();
        private Stats m_FinalStats;

        private Quaternion m_LastValidRotation;
        private Vector3 m_LastCollisionNormal;
        private bool m_HasCollision;
        private bool m_InAir = false;

        public void AddPowerup(StatPowerup statPowerup) => m_ActivePowerupList.Add(statPowerup);
        public void SetCanMove(bool move) => m_CanMove = move;
        public float GetMaxSpeed() => Mathf.Max(m_FinalStats.TopSpeed, m_FinalStats.ReverseSpeed);

        private void ActivateDriftVFX(bool active)
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

        void UpdateSuspensionParams(WheelCollider wheel)
        {
            wheel.suspensionDistance = _suspensionHeight;
            wheel.center = new Vector3(0.0f, _wheelsPositionVerticalOffset, 0.0f);
            JointSpring spring = wheel.suspensionSpring;
            spring.spring = _suspensionSpring;
            spring.damper = _suspensionDamp;
            wheel.suspensionSpring = spring;
        }

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            UpdateSuspensionParams(_wheelColliderFrontLeft);
            UpdateSuspensionParams(_wheelColliderFrontRight);
            UpdateSuspensionParams(_wheelColliderRearLeft);
            UpdateSuspensionParams(_wheelColliderRearRight);

            m_CurrentGrip = _baseStats.Grip;

            if (_enableParticles)
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

        void FixedUpdate()
        {
            UpdateSuspensionParams(_wheelColliderFrontLeft);
            UpdateSuspensionParams(_wheelColliderFrontRight);
            UpdateSuspensionParams(_wheelColliderRearLeft);
            UpdateSuspensionParams(_wheelColliderRearRight);

            _wantsToDrift = _input.Value.InputData.AccelerateInput < 0;// && Vector3.Dot(_rigidbody.velocity, transform.forward) > 0.0f;

            // apply our powerups to create our finalStats
            TickPowerups();

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

            // calculate how grounded and airborne we are
            _groundPercent = (float) groundedCount / 4.0f;
            _airPercent = 1 - _groundPercent;

            // apply vehicle physics
            if (m_CanMove)
            {
                MoveVehicle(_input.Value.InputData.AccelerateInput > 0, _input.Value.InputData.AccelerateInput < 0, _input.Value.InputData.SteerInput);
            }
            GroundAirbourne();

            m_PreviousGroundPercent = _groundPercent;

            if (_enableParticles)
                UpdateDriftVFXOrientation();
        }

        void TickPowerups()
        {
            // remove all elapsed powerups
            m_ActivePowerupList.RemoveAll((p) => { return p.ElapsedTime > p.MaxTime; });

            // zero out powerups before we add them all up
            var powerups = new Stats();

            // add up all our powerups
            for (int i = 0; i < m_ActivePowerupList.Count; i++)
            {
                var p = m_ActivePowerupList[i];

                // add elapsed time
                p.ElapsedTime += Time.fixedDeltaTime;

                // add up the powerups
                powerups += p.modifiers;
            }

            // add powerups to our final stats
            m_FinalStats = _baseStats + powerups;

            // clamp values in finalstats
            m_FinalStats.Grip = Mathf.Clamp(m_FinalStats.Grip, 0, 1);
        }

        void GroundAirbourne()
        {
            // while in the air, fall faster
            if (_airPercent >= 1)
            {
                _rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime * m_FinalStats.AddedGravity;
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
                    return dot < 0 ? -(speed / m_FinalStats.ReverseSpeed) : (speed / m_FinalStats.TopSpeed);
                }
                return 0f;
            }
            else
            {
                // use this value to play kart sound when it is waiting the race start countdown.
                return _input.Value.InputData.AccelerateInput;
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
            float maxSpeed = localVelDirectionIsFwd ? m_FinalStats.TopSpeed : m_FinalStats.ReverseSpeed;
            float accelPower = accelDirectionIsFwd ? m_FinalStats.Acceleration : m_FinalStats.ReverseAcceleration;

            float currentSpeed = _rigidbody.velocity.magnitude;
            float accelRampT = currentSpeed / maxSpeed;
            float multipliedAccelerationCurve = m_FinalStats.AccelerationCurve * accelerationCurveCoeff;
            float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

            bool isBraking = (localVelDirectionIsFwd && brake) || (!localVelDirectionIsFwd && accelerate);

            // if we are braking (moving reverse to where we are going)
            // use the braking accleration instead
            float finalAccelPower = isBraking ? m_FinalStats.Braking : accelPower;

            float finalAcceleration = finalAccelPower * accelRamp;

            // apply inputs to forward/backward
            float turningPower = IsDrifting ? m_DriftTurningPower : turnInput * m_FinalStats.Steer;

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
                newVelocity = Vector3.MoveTowards(newVelocity, new Vector3(0, _rigidbody.velocity.y, 0), Time.fixedDeltaTime * m_FinalStats.CoastingDrag);
            }

            _rigidbody.velocity = newVelocity;

            // Drift
            if (_groundPercent > 0.0f)
            {
                if (m_InAir)
                {
                    m_InAir = false;

                    if (_enableParticles)
                        Instantiate(_jumpVFX, transform.position, Quaternion.identity);
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
                if (_groundPercent >= 0.0f && m_PreviousGroundPercent < 0.1f)
                {
                    Vector3 flattenVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, m_VerticalReference).normalized;
                    if (Vector3.Dot(flattenVelocity, transform.forward * Mathf.Sign(accelInput)) < Mathf.Cos(_driftStats.MinAngleToFinishDrift * Mathf.Deg2Rad))
                    {
                        IsDrifting = true;
                        m_CurrentGrip = _driftStats.DriftGrip;
                        m_DriftTurningPower = 0.0f;
                    }
                }

                // Drift Management
                if (!IsDrifting)
                {
                    if ((_wantsToDrift || isBraking) && currentSpeed > maxSpeed * _driftStats.MinSpeedPercentToFinishDrift)
                    {
                        IsDrifting = true;
                        m_DriftTurningPower = turningPower + (Mathf.Sign(turningPower) * _driftStats.DriftAdditionalSteer);
                        m_CurrentGrip = _driftStats.DriftGrip;

                        if (_enableParticles)
                            ActivateDriftVFX(true);
                    }
                }

                if (IsDrifting)
                {
                    float turnInputAbs = Mathf.Abs(turnInput);
                    if (turnInputAbs < k_NullInput)
                        m_DriftTurningPower = Mathf.MoveTowards(m_DriftTurningPower, 0.0f, Mathf.Clamp01(_driftStats.DriftDampening * Time.fixedDeltaTime));

                    // Update the turning power based on input
                    float driftMaxSteerValue = m_FinalStats.Steer + _driftStats.DriftAdditionalSteer;
                    m_DriftTurningPower = Mathf.Clamp(m_DriftTurningPower + (turnInput * Mathf.Clamp01(_driftStats.DriftControl * Time.fixedDeltaTime)), -driftMaxSteerValue, driftMaxSteerValue);

                    bool facingVelocity = Vector3.Dot(_rigidbody.velocity.normalized, transform.forward * Mathf.Sign(accelInput)) > Mathf.Cos(_driftStats.MinAngleToFinishDrift * Mathf.Deg2Rad);

                    bool canEndDrift = true;
                    if (isBraking)
                        canEndDrift = false;
                    else if (!facingVelocity)
                        canEndDrift = false;
                    else if (turnInputAbs >= k_NullInput && currentSpeed > maxSpeed * _driftStats.MinSpeedPercentToFinishDrift)
                        canEndDrift = false;

                    if (canEndDrift || currentSpeed < k_NullSpeed)
                    {
                        // No Input, and car aligned with speed direction => Stop the drift
                        IsDrifting = false;
                        m_CurrentGrip = m_FinalStats.Grip;
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
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime * (_groundPercent > 0.0f ? 10.0f : 1.0f)));    // Blend faster if on ground
            }
            else
            {
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > 0.0f) ? m_LastCollisionNormal : Vector3.up;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime));
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
                    _rigidbody.MoveRotation(Quaternion.Lerp(_rigidbody.rotation, Quaternion.LookRotation(finalOrientationDirection, m_VerticalReference), Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime)));
                }
            }
            else if (validPosition)
            {
                m_LastValidRotation.eulerAngles = new Vector3(0.0f, transform.rotation.y, 0.0f);
            }

            if (_enableParticles)
                ActivateDriftVFX(IsDrifting && _groundPercent > 0.0f);
        }
    }
}
