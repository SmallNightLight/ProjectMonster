using System.Collections.Generic;
using UnityEngine;

namespace ScriptableArchitecture.Core
{
    public class InstanceScope : MonoBehaviour
    {
        private Dictionary<Variable, Variable> _instancedVariables = new Dictionary<Variable, Variable>();

        public Variable GetOrInstanceVariable(Variable baseVariable)
        {
            if (_instancedVariables.ContainsKey(baseVariable))
            {
                //Instance in scope already instanced
                return _instancedVariables[baseVariable];
            }
            else
            {
                //Create new temporary variable
                var tempVariable = Instantiate(baseVariable);
                _instancedVariables.Add(baseVariable, tempVariable);
                return tempVariable;
            }
        }
    }
}