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
    }

    public void Close()
    {
        foreach (GameObject effectObject in _effectObjects)
        {
            GameObject.Destroy(effectObject);
        }
    }
}