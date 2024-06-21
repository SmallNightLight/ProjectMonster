using ScriptableArchitecture.Core;
using UnityEngine;

public class IgnoreKarts : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Receiver _allKarts;

    private void Start()
    {
        HitTrigger hitTrigger = GetComponentInParent<HitTrigger>();

        if (hitTrigger == null) return;

        foreach (KartBase kartBase in _allKarts.GetAllValues<KartBase>())
        {
            if (kartBase.Player == hitTrigger.FromPlayer) continue;

            foreach(Collider other in kartBase.gameObject.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(_collider, other);
            }
        }
    }
}