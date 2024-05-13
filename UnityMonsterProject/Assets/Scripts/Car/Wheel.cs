using ScriptableArchitecture.Data;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private InputDataReference _inputData;
    [SerializeField] private LayerMaskReference _drivingLayers;
    [SerializeField] private bool _steeringWheel;

    [Header("Acceleration")]
    [SerializeField] private FloatReference _carMaxSpeed;
    [SerializeField] private AnimationCurveReference _accelerationCurve;

    [Header("Steering")]
    [SerializeField] private FloatReference _tireGripFactor;
    [SerializeField] private FloatReference _tireMass;
    [SerializeField] private FloatReference _steeringLimit;

    [Header("Suspension")]
    [SerializeField] private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);
    [SerializeField] private FloatReference _springStrength;
    [SerializeField] private FloatReference _springDamping;
    [SerializeField] private FloatReference _suspensionRestDistance;
    [SerializeField] private FloatReference _wheelHeight;

    [Space]
    [Header("Components")]
    [SerializeField] private CarController _carController;
    [SerializeField] private Transform _carTransform;
    [SerializeField] private Rigidbody _carRigidbody;

    [Header("Debug")]
    [SerializeField] bool _drawAcceleration;
    [SerializeField] bool _drawSteering;
    [SerializeField] bool _drawSuspension;

    [Header("Forces")]
    private float _suspension;
    private float _accceleration;
    private float _steering;

    private float _targetSteering;
    private float _currentSteering;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (_steeringWheel)
            UpdateSteering();
    }

    private void UpdateSteering()
    {
        float steeringInput = _inputData.Value.Horizontal;

        _targetSteering = steeringInput * _steeringLimit.Value;

        _currentSteering = Mathf.Lerp(_currentSteering, _targetSteering, Time.deltaTime * 4f);
        transform.eulerAngles = new Vector3(0, _currentSteering, 0);
    }


    //Physics

    private void FixedUpdate()
    {
        CalculateSuspension();
    }

    private void CalculateSuspension()
    {
        bool rayHit = Physics.Raycast(transform.position, _gravity, out RaycastHit hit, _wheelHeight.Value, _drivingLayers.Value);

        
        float accelerationInput = _inputData.Value.Vertical;

        //Suspension
        if (rayHit)
        {
            Vector3 springDirection = transform.up;
            Vector3 tireWorldVelocity = _carRigidbody.GetPointVelocity(transform.position);

            float offset = _suspensionRestDistance.Value - hit.distance;

            float velocity = Vector3.Dot(springDirection, tireWorldVelocity);

            float force = (offset * _springStrength.Value) - (velocity * _springDamping.Value);

            _carRigidbody.AddForceAtPosition(springDirection * force, transform.position);

            if (_drawSuspension)
            {
                Debug.DrawLine(transform.position, transform.position + (springDirection * force), Color.blue);
                Debug.DrawLine(transform.position, transform.position + _gravity, Color.green);
            }
        }

        //Steering
        if (rayHit)
        {
            Vector3 steeringDirection = transform.right;
            Debug.Log(steeringDirection);
            Vector3 tireWorldVelocity = _carRigidbody.GetPointVelocity(transform.position);

            float steeringVelocity = Vector3.Dot(steeringDirection, tireWorldVelocity);

            float desiredVelocityChange = -steeringVelocity * _tireGripFactor.Value;
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

            _carRigidbody.AddForceAtPosition(steeringDirection * _tireMass.Value * desiredAcceleration, transform.position);
        }

        //Acceleration
        if (rayHit)
        {
            if (accelerationInput != 0.0f)
            {
                Vector3 direction = transform.forward;

                //float carSpeed = Vector3.Dot(_carTransform.forward, _carRigidbody.velocity);
                //float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / _carMaxSpeed.Value);
                //float availableTorque = _accelerationCurve.Value.Evaluate(normalizedSpeed) * accelerationInput;

                float availableTorque = _carMaxSpeed.Value * accelerationInput;

                _carRigidbody.AddForceAtPosition(direction * availableTorque, transform.position);

                

                if (_drawAcceleration)
                {
                    //Debug.Log("CarSpeed: " + carSpeed);
                    //Debug.Log("normalizedSpeed: " + normalizedSpeed);
                    //Debug.Log("AvailableTorque: " + availableTorque);
                }
            }
        }
    }
}