using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KartBase), typeof(KartMovement))]
public class KartAbilities : MonoBehaviour
{
    [Header("Abilities")]
    [SerializeField] private AbilityDataReference _boostAbility;
    [SerializeField] private AbilityDataReference _ability1;
    

    [Header("Components")]
    private KartBase _base;
    private KartMovement _kartMovement;

    private List<ActiveAbility> _activeAbilities = new List<ActiveAbility>();

    private float _boostCoolDownTimer;
    private float _ability1CoolDownTimer;

    private bool _canDoBoost;
    private bool _canDoAbility1;

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
            AddAbility(_boostAbility.Value);
            _canDoBoost = false;
        }

        if (_base.Input.Ability1 && _canDoAbility1)
        {
            AddAbility(_ability1.Value);
            _canDoAbility1 = false;
        }
    }

    private void UpdateCoolDown()
    {
        if (!_canDoBoost)
        {
            _boostCoolDownTimer += Time.deltaTime;

            if (_boostCoolDownTimer > _boostAbility.Value.CoolDown)
            {
                _canDoBoost = true;
                _boostCoolDownTimer = 0f;
            }
        }

        if (!_canDoAbility1)
        {
            _ability1CoolDownTimer += Time.deltaTime;

            if (_ability1CoolDownTimer > _ability1.Value.CoolDown)
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

            //Set the parent if specified
            if (worldEffect.KartIsParent)
            {
                instance.transform.SetParent(transform);
            }

            if (worldEffect.UseKartPosition)
            {
                instance.transform.position = transform.position + worldEffect.Position;
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
            else if (worldEffect.UseIdentityRotation)
            {
                instance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                instance.transform.localRotation = Quaternion.Euler(worldEffect.Rotation);
            }
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
}