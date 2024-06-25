using Cinemachine;
using ScriptableArchitecture.Data;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BaseEffect))]
public class AbilityFireball : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] private float _power;
    [SerializeField] private float _distancePower;
    [SerializeField] private float _gravity;

    [SerializeField] private StringReference _groundTagName;

    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private float _impactDuration = 2f;
    [SerializeField] private float _checkoffset = 1;
    [SerializeField] private float _hitOffset = 1;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private Vector3 _impactOffset;

    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private bool _isExploded;

    private BaseEffect _baseEffect;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _baseEffect = GetComponent<BaseEffect>();

        float firePower = _power + _distancePower * _baseEffect.KartSpeed;
        _rigidbody.AddForce(transform.forward * firePower, ForceMode.Impulse);

        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        _rigidbody.AddForce(new Vector3(0, _gravity , 0) * _rigidbody.mass);
    }

    private Vector3 lastPosition;

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 direction = (currentPosition - lastPosition).normalized;
        float distance = Vector3.Distance(currentPosition, lastPosition);

        RaycastHit hit;
        if (Physics.Raycast(lastPosition, direction, out hit, distance))
        {
            if (hit.collider.gameObject.tag == _groundTagName.Value && !_isExploded)
            {
                Explode(hit.point);
            }
        }

        lastPosition = currentPosition;
    }

    private void Explode(Vector3 impactPosition)
    {
        _isExploded = true;
        GameObject impactEffect = Instantiate(_impactPrefab, impactPosition + _impactOffset, Quaternion.identity);
        Destroy(impactEffect, _impactDuration);

        HitTrigger hitTriger = impactEffect.GetComponentInChildren<HitTrigger>();
        if (hitTriger != null)
            hitTriger.FromPlayer = _baseEffect.FromPlayer;

        if (_impulseSource != null)
            _impulseSource.GenerateImpulseAt(transform.position, Vector3.up);
    }
}