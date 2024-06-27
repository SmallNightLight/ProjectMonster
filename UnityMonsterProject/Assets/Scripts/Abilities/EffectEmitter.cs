using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BaseEffect))]
public class EffectEmitter : MonoBehaviour
{
    private BaseEffect _baseEffect;

    [SerializeField] private float _startDelay;
    [SerializeField] private float _interval;
    [SerializeField] private float _count;

    [SerializeField] private GameObject _effectPrefab;
    [SerializeField] private float _effectDuration = 2f;

    public IEnumerator Start()
    {
        _baseEffect = GetComponent<BaseEffect>();

        yield return new WaitForSeconds(_startDelay);

        for(int i = 0; i < _count; ++i)
        {
            Spawn();
            yield return new WaitForSeconds(_interval);
        }
    }

    public void Spawn()
    {
        GameObject effect = Instantiate(_effectPrefab, transform.position, transform.rotation);
        Destroy(effect, _effectDuration);

        if (effect.TryGetComponent(out BaseEffect baseEffect))
        {
            baseEffect.KartSpeed = _baseEffect.KartSpeed;
            baseEffect.FromPlayer = _baseEffect.FromPlayer;
        }

        HitTrigger hitTriger = effect.GetComponentInChildren<HitTrigger>();
        if (hitTriger != null)
            hitTriger.FromPlayer = _baseEffect.FromPlayer;
    }
}