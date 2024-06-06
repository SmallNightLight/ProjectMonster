using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AbilityFireball : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] private float _power;
    [SerializeField] private float _gravity;

    [SerializeField] private string _hitTag;

    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private float _checkoffset = 1;

    private bool _isExploded;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(transform.forward * _power, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        _rigidbody.AddForce(new Vector3(0, _gravity , 0) * _rigidbody.mass);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == _hitTag && !_isExploded)
        {
            Vector3 rayStartPoint = transform.position + new Vector3(0, _checkoffset, 0);

            RaycastHit hit;
            if (Physics.Raycast(rayStartPoint, Vector3.down, out hit))
            {
                Explode(hit.point);
            }
        }
    }

    private void Explode(Vector3 impactPosition)
    {
        _isExploded = true;
        GameObject impactEffect = Instantiate(_impactPrefab, impactPosition, Quaternion.identity);
        Destroy(impactEffect, 2);
    }
}