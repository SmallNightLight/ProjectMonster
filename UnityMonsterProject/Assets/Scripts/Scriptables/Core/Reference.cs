using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableArchitecture.Core
{
    /// <summary>
    /// Generic class representing a reference to either a variable or a constant value of type T.
    /// If the variable is null or the when the constant is sed, the Value returns the constant, 
    /// otherwise a Variable of the same genric type is returned.
    /// Implements functions from the variable for the variable, event and runtimeset
    /// </summary>
    [Serializable]
    public class Reference<T, TVariable> where TVariable : Variable<T>
    {
        [SerializeField] protected bool _isVariable;
        [SerializeField] protected TVariable _variable;
        [SerializeField] protected T _constant;

        //Variable

        /// <summary>
        /// Gets or sets the value of the reference based whether the variable is chosen or null
        /// </summary>
        public T Value
        {
            get
            {
                if (_isVariable && _variable != null)
                    return _variable.Value;
                else
                    return _constant;
            }
            set
            {
                if (_isVariable && _variable != null)
                    _variable.Value = value;
                else
                {
                    _constant = value;
                    _isVariable = false;
                }
            }
        }

        /// <summary>
        /// Use this function to override the variable
        /// </summary>
        /// <param name="newVariable"></param>
        public void OverrideVariable(TVariable newVariable)
        {
            _variable = newVariable;
        }

        /// <summary>
        /// Use this function to override whether the reference is a constant or a variable
        /// </summary>
        /// <param name="isVariable"></param>
        public void OverrideIsVariable(bool isVariable)
        {
            _isVariable = isVariable;
        }

        //Event

        /// <summary>
        /// Raises the event with a value from the same generic type on the variable. 
        /// Doesn't do anything when a constant is used
        /// </summary>
        public void Raise(T value)
        {
            if (_isVariable)
                _variable?.Raise(value);
        }

        /// <summary>
        /// Raises the event on the variable. Doesn't do anything when a constant is used
        /// </summary>
        public void Raise()
        {
            if (_isVariable)
                _variable?.Raise();
        }


        //RuntimeSet

        /// <summary>
        /// Gets the runtime set of the variable. This is the actual reference, 
        /// for modifying it prefer the build in functions on the reference like Add, Remove, and more
        /// WHen set to constant it returns a new list with the constant value
        /// </summary>
        public List<T> RuntimeSet
        {
            get
            {
                if (_isVariable && _variable != null)
                    return _variable.RuntimeSet;

                return new List<T> { _constant };
            }
        }

        /// <summary>
        /// When a variable adds the given value to the runtimeset
        /// </summary>
        public void Add(T value)
        {
            if (_isVariable)
                _variable?.Add(value);

        }

        /// <summary>
        /// When a variable removes the given value from the runtimeset
        /// </summary>
        public void Remove(T value)
        {
            if (_isVariable)
                _variable?.Remove(value);
        }

        /// <summary>
        /// When a variable, clears the runtimeset
        /// </summary>
        public void Clear()
        {
            if (_isVariable)
                _variable?.ClearRuntimeSet();
        }

        /// <summary>
        /// When a variable, checks whether the runtimeset contains the value.
        /// Returns false when a constant
        /// </summary>
        public bool HasItem(T value)
        {
            if (_isVariable && _variable != null)
                return _variable.RuntimeSet.Contains(value);

            return false;
        }
    }
}