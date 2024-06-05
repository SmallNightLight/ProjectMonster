using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KartBase), typeof(KartMovement))]
public class KartAbilities : MonoBehaviour
{
    [Header("Abilities")]
    [SerializeField] private AbilityDataReference _boostAbility;
    [SerializeField] private AbilityDataReference _mainAbility;
    

    [Header("Components")]
    private KartBase _base;
    private KartMovement _kartMovement;

    private List<ActiveAbility> _activeAbilities = new List<ActiveAbility>();


    private void Start()
    {
        _base = GetComponent<KartBase>();
        _kartMovement = GetComponent<KartMovement>();
    }

    private void Update()
    {
        UpdateAbilities();

        if (_base.Input.AbilityBoost)
        {
            AddAbility(_boostAbility.Value);
        }

        if (_base.Input.Ability1)
        {
            AddAbility(_mainAbility.Value);
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

            //Set the position
            instance.transform.position = worldEffect.Position;

            //Set the rotation
            if (worldEffect.UseIdentityRotation)
            {
                instance.transform.rotation = Quaternion.identity;
            }
            else
            {
                instance.transform.rotation = Quaternion.Euler(worldEffect.Rotation);
            }

            //Set the parent if specified
            if (worldEffect.KartIsParent)
            {
                instance.transform.SetParent(transform);
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