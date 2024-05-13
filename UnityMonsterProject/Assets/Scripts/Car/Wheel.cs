using ScriptableArchitecture.Data;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private InputDataReference _inputData;
    [SerializeField] private LayerMaskReference _drivingLayers;
    [SerializeField] private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);
    [SerializeField] private FloatReference _springStrength;
    [SerializeField] private FloatReference _springDamping;
    [SerializeField] private FloatReference _suspensionRestDistance;
    [SerializeField] private FloatReference _wheelHeight;

    [Header("Componnets")]
    [SerializeField] private CarController _carController;
    [SerializeField] private Rigidbody _carRigidbody;

    [Header("Forces")]
    private float _suspension;
    private float _accceleration;
    private float _steering;

    private void Start()
    {
        
    }

    private void Update()
    {
        CalculateSuspension();
    }

    private void CalculateSuspension()
    {
        bool rayHit = Physics.Raycast(transform.position, _gravity, out RaycastHit hit, _wheelHeight.Value, _drivingLayers.Value);

        //Acceleration
        if (rayHit)
        {
            Vector3 direction = transform.forward;
        }

        //Suspension
        if (rayHit)
        {
            Vector3 springDirection = transform.up;
            Vector3 tireWorldVelocity = _carRigidbody.GetPointVelocity(transform.position);

            float offset = _suspensionRestDistance.Value - hit.distance;

            float velocity = Vector3.Dot(springDirection, tireWorldVelocity);

            float force = (offset * _springStrength.Value) - (velocity * _springDamping.Value);

            _carRigidbody.AddForceAtPosition(springDirection * force, transform.position);

            //Draw debug lines
            Debug.DrawLine(transform.position, transform.position + (springDirection * force), Color.blue);
            Debug.DrawLine(transform.position, transform.position + _gravity, Color.green);
        }

        //Steering
        if (rayHit)
        {

        }


    }
}