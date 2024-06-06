using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AbilityFireball : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] private float _power;
    [SerializeField] private float _gravity;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(transform.forward * _power, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        _rigidbody.AddForce(new Vector3(0, _gravity , 0) * _rigidbody.mass);
    }

}