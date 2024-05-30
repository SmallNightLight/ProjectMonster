using ScriptableArchitecture.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActiveAbility
{
    public AbilityData AbilityData;
    public float ElapsedTime;

    private List<GameObject> _effectObjects;

    public ActiveAbility(AbilityData abilityData, float elapsedTime)
    {
        AbilityData = abilityData;
        ElapsedTime = elapsedTime;

        foreach (WorldEffect worldEffect in abilityData.WorldEffects)
        {
            if (worldEffect == null) continue;

            // Instantiate the prefab
            //GameObject instance = Instantiate(effect.Prefab);

            //// Set the position
            //instance.transform.position = effect.Position;

            //// Set the rotation
            //if (effect.UseIdentityRotation)
            //{
            //    instance.transform.rotation = Quaternion.identity;
            //}
            //else
            //{
            //    instance.transform.rotation = Quaternion.Euler(effect.Rotation);
            //}

            //// Set the parent if specified
            //if (effect.HasParent && effect.Parent != null)
            //{
            //    instance.transform.SetParent(effect.Parent.transform);
            //}
        }
    }

    public void Close()
    {
        foreach (GameObject effectObject in _effectObjects)
        {
            GameObject.Destroy(effectObject);
        }
    }
}