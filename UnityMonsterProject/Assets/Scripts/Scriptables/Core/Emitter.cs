using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptableArchitecture.Core
{
    /// <summary>
    /// Emits values to a receiver with a priority, higher priority means it has priority over other values of other emmiters with the same receiver
    /// </summary>
    public class Emitter : MonoBehaviour
    {
        [SerializeField] private Receiver _receiver;
        [SerializeField] private Object _value;

        [SerializeField] private bool _activateOnStart = true;

        [Tooltip("Higher number for more important emmiters")]
        [SerializeField] private int _priority = 0;

        /// <summary>
        /// Gets or sets the priority of the emitte (higher number for more important emitters)
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
                _receiver?.UpdatePriorityList(this);
            }
        }

        private bool _isAdded;

        /// <summary>
        /// Gets the value of and tries to convert it to the specified type
        /// </summary>
        public T Value<T>() where T : Object
        {
            if (_value is T)
                return _value as T;

            Debug.LogError($"Could not convert from \"{_value.GetType()} to {typeof(T)} ");
            Debug.Break();
            return null;
        }

        /// <summary>
        /// Sets the value to the given value
        /// </summary>
        public void SetValue<T>(T value)
        {
            _value = value as Object;
        }

        /// <summary>
        /// Sets the value to the given object value
        /// </summary>
        public void SetValue(Object value)
        {
            _value = value;
        }

        /// <summary>
        /// Registeres the emmiter when it is enabled, when this funtionality is activated (_activateOnStart)
        /// </summary>
        private void OnEnable()
        {
            if (_activateOnStart)
                Register();
        }

        /// <summary>
        /// Unregisteres the emmiter when it is disabled
        /// </summary>
        private void OnDisable()
        {
            Unregister();
        }

        /// <summary>
        /// Adds the emmiter to the receiver
        /// </summary>
        public void Register()
        {
            _isAdded = true;
            _receiver.RegisterEmitter(this);
        }

        /// <summary>
        /// Removes the emmiter to the receiver
        /// </summary>
        public void Unregister()
        {
            if (_isAdded)
                _receiver?.UnregisterEmitter(this);

            _isAdded = false;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Updates the priority on the receiver when a change is detected
        /// </summary>
        private void OnValidate()
        {
            _receiver?.UpdatePriorityList(this);
        }
#endif
    }
}