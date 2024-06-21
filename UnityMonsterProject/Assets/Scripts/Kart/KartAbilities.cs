using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(KartBase), typeof(KartMovement))]
public class KartAbilities : MonoBehaviour
{
    [Header("Ability")]
    [SerializeField] private AbilityDataReference _hitAbiity;

    [Header("Events")]
    [SerializeField] private UnityEvent _hitEvent;
    [SerializeField] private UnityEvent _abilityEvent;

    [Header("Components")]
    private KartBase _base;
    private KartMovement _kartMovement;

    private List<ActiveAbility> _activeAbilities = new List<ActiveAbility>();

    private float _boostCoolDownTimer;
    private float _ability1CoolDownTimer;

    private bool _canDoBoost;
    private bool _canDoAbility1;

    [SerializeField] private FloatReference _boostCoolDown;
    [SerializeField] private FloatReference _abilityCoolDown;

    private void Start()
    {
        _base = GetComponent<KartBase>();
        _kartMovement = GetComponent<KartMovement>();

        _canDoBoost = true;
        _canDoAbility1 = true;

        _boostCoolDownTimer = 0f;
        _ability1CoolDownTimer = 0f;
    }

    private void Update()
    {
        UpdateAbilities();
        UpdateCoolDown();

        if (_base.Input.AbilityBoost && _canDoBoost)
        {
            AddAbility(_base.CharacterData.BoostAbility.Value);
            _canDoBoost = false;
        }

        if (_base.Input.Ability1 && _canDoAbility1)
        {
            AddAbility(_base.CharacterData.MainAbility.Value);
            _abilityEvent.Invoke();
            _canDoAbility1 = false;
        }
    }

    private void UpdateCoolDown()
    {
        if (!_canDoBoost)
        {
            _boostCoolDownTimer += Time.deltaTime;
            _boostCoolDown.Value = 1 - _boostCoolDownTimer / _base.CharacterData.BoostAbility.Value.CoolDown;

            if (_boostCoolDownTimer > _base.CharacterData.BoostAbility.Value.CoolDown)
            {
                _canDoBoost = true;
                _boostCoolDownTimer = 0f;
            }
        }

        if (!_canDoAbility1)
        {
            _ability1CoolDownTimer += Time.deltaTime;
            _abilityCoolDown.Value = 1 - _ability1CoolDownTimer / _base.CharacterData.MainAbility.Value.CoolDown;

            if (_ability1CoolDownTimer > _base.CharacterData.MainAbility.Value.CoolDown)
            {
                _canDoAbility1 = true;
                _ability1CoolDownTimer = 0f;
            }
        }
    }

    public void AddAbility(AbilityData effect)
    {
        _activeAbilities.Add(new ActiveAbility(effect, 0));
        _kartMovement.AddAbility(effect);

        foreach (WorldEffect worldEffect in effect.WorldEffects)
        {
            if (worldEffect == null) continue;

            //Instantiate the prefab
            GameObject instance = Instantiate(worldEffect.Prefab);

            if (instance.TryGetComponent(out BaseEffect baseEffect))
            {
                //Set some values
                baseEffect.KartSpeed = _kartMovement.LocalSpeed();
            }

            Destroy(instance, effect.Duration);

            //Set the parent if specified
            if (worldEffect.KartIsParent)
            {
                instance.transform.SetParent(transform);
            }

            if (worldEffect.UseKartPosition)
            {
                instance.transform.position = transform.position + transform.rotation * worldEffect.Position;
            }
            else if (worldEffect.UseKartPositionXZ)
            {
                instance.transform.position = new Vector3(transform.position.x, 0, transform.position.z) + transform.rotation * worldEffect.Position;
            }
            else
            {
                //Set the position
                instance.transform.localPosition = worldEffect.Position;
            }

            if (worldEffect.UseKartRotation)
            {
                instance.transform.rotation = transform.rotation * Quaternion.Euler(worldEffect.Rotation);
            }
            else if (worldEffect.UseKartRotationY)
            {
                instance.transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)) * Quaternion.Euler(new Vector3(0, worldEffect.Rotation.y, 0));
            }
            else if (worldEffect.UseIdentityRotation)
            {
                instance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                instance.transform.localRotation = Quaternion.Euler(worldEffect.Rotation);
            }

            //Set player to hittrigger
            HitTrigger hitTriger = instance.GetComponentInChildren<HitTrigger>();
            if (hitTriger != null)
                hitTriger.FromPlayer = _base.Player;
        }
    }

    void UpdateAbilities()
    {
        for (int i = _activeAbilities.Count - 1; i >= 0; i--)
        {
            _activeAbilities[i].ElapsedTime += Time.fixedDeltaTime;

            if (_activeAbilities[i].ElapsedTime >= _activeAbilities[i].AbilityData.Duration)
            {
                _kartMovement.RemoveAbility(_activeAbilities[i].AbilityData);
                _activeAbilities.RemoveAt(i);
            }   
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hit")
        {
            HitTrigger hitTriger = other.gameObject.GetComponentInChildren<HitTrigger>();
            if (hitTriger != null && hitTriger.FromPlayer == _base.Player) return;

            AddAbility(_hitAbiity.Value);
            _hitEvent.Invoke();
        }
    }
}