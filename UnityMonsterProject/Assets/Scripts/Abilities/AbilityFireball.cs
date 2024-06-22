using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BaseEffect))]
public class AbilityFireball : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] private float _power;
    [SerializeField] private float _distancePower;
    [SerializeField] private float _gravity;

    [SerializeField] private string _hitTag;

    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private float _impactDuration = 2f;
    [SerializeField] private float _checkoffset = 1;
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
            if (Physics.Raycast(rayStartPoint, Vector3.down, out hit, _checkoffset * 2, _hitMask, QueryTriggerInteraction.Ignore))
            {
                Explode(hit.point);
            }
        }
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