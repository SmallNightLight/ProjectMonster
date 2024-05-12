using UnityEngine;

public class KartController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _steeringAmount;
    [SerializeField] private float _gravity = 9.81f;
   
    [Header("Components")]
    [SerializeField] private Rigidbody _sphereRigidbody;
    [SerializeField] private Transform _kart;

    private float _speed, _rotation;
    private float _currentSpeed, _currentRotation;

    private void Update()
    {
        transform.position = _sphereRigidbody.transform.position - _offset;

        if (Input.GetKeyDown(KeyCode.W))
            _speed = _acceleration;

        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            int direction = horizontalInput > 0 ? 1 : -1;
            float amount = Mathf.Abs(horizontalInput);
            Steer(direction, amount);
        }

        _currentSpeed = Mathf.SmoothStep(_currentSpeed, _speed, Time.deltaTime * 12f);
        _currentRotation = Mathf.Lerp(_currentRotation, _rotation, Time.deltaTime * 4f);

        _speed = 0f;
        _rotation = 0f;
    }

    private void FixedUpdate()
    {
        _sphereRigidbody.AddForce(_kart.transform.forward * _currentSpeed, ForceMode.Acceleration);

        //Gravity
        _sphereRigidbody.AddForce(Vector3.down, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + _currentRotation, 0), Time.deltaTime * 5f);

        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitOn, 1.1f);
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitNear, 2.0f);

        //Kart rotation
        _kart.parent.up = Vector3.Lerp(_kart.parent.up, hitNear.normal, Time.deltaTime * 8f);
        _kart.parent.Rotate(0, transform.eulerAngles.y, 0);
    }

    private void Steer(int direction, float amount)
    {
        _rotation = _steeringAmount * direction * amount;
    }
}