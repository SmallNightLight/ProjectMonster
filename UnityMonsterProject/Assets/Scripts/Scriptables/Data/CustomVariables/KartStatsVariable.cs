using PlasticPipe.PlasticProtocol.Messages;
using ScriptableArchitecture.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    public class KartStatsVariable<T> : KartStatsVariable, IGameEvent<T> where T : IStats
    {
        //Variable
        [SerializeField] private T _value;
        [SerializeField] private T _startValue;

        /// <summary>
        /// Gets or sets the value of the variable. Doen't set the variable when readonly.
        /// Gets the startvalue when readonly
        /// </summary>
        public T Value
        {
            get
            {
                if (InitializeTypeVariable == InitializeType.ReadOnly)
                    return _startValue;

                return _value;
            }
            set
            {
                if (InitializeTypeVariable == InitializeType.ReadOnly)
                {
                    Debug.LogWarning($"Cannot set: {name} (Readonly)");
                    return;
                }

                _value = value;
                Log(_stacktraceVariable, $"Set value to: {Value}");
            }
        }

        public override IStats GetStats()
        {
            return Value;
        }


        //Event
        private List<IListener<T>> _listeners;


        //RuntimeSet


        [SerializeField] private List<T> _runtimeSet;
        [SerializeField] private List<T> _startRuntimeSet;

        /// <summary>
        /// Gets the runtimeset. When readonly gets the start runtimeset
        /// </summary>
        public List<T> RuntimeSet
        {
            get
            {
                if (InitializeTypeRuntimeSet == InitializeType.ReadOnly)
                    return _startRuntimeSet;

                return _runtimeSet;
            }
        }


        /// <summary>
        /// Initializes the values on game start
        /// This function is called when the first scene is loaded, regardles if the scene has a reference to it (build game)
        /// </summary>
        private void OnEnable()
        {
            if (VariableType == VariableType.Variable || VariableType == VariableType.VariableEvent)
            {
                //This is only for the editor as the build game does not save Scriptable objects across sessions
                if (InitializeTypeVariable == InitializeType.ResetOnGameStart)
                {
                    if (_startValue is IAssignment<T>)
                    {
                        //Has a custom copy function (with the IAssignemnt interface)
                        _value = (_startValue as IAssignment<T>).Copy();
                    }
                    else
                    {
                        _value = _startValue;
                    }

                    Log(_stacktraceVariable, $"Set value to startvalue: {Value}");
                }
            }

            if (VariableType == VariableType.Event || VariableType == VariableType.VariableEvent)
            {
                //Only initilize lists when needed to avoid overhead
                InitializeListeners();
            }

            if (VariableType == VariableType.RuntimeSet)
            {
                if (InitializeTypeRuntimeSet == InitializeType.ResetOnGameStart)
                {
                    _runtimeSet = new List<T>(_startRuntimeSet);
                    Log(_stacktraceRuntimeSet, $"Set runtimeSet to startvalue");
                }

                if (InitializeTypeRuntimeSet == InitializeType.Normal)
                    InitializeRuntimeSet();
            }

            _stacktraceEvent.Clear();
        }


        //Variable

        /// <summary>
        /// Function for settings the value by function. Use this for unity events
        /// </summary>
        public void Set(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Overides the startvalue immediatly, use this function with caution as this might cause issues in the build.
        /// Only use this for variable creation
        /// </summary>
        /// <param name="value"></param>
        public void SetStartValueImmediatly(T value)
        {
            _startValue = value;
            Log(_stacktraceVariable, $"Set start value to {_startValue}");
        }

        /// <summary>
        /// Resets the variable values to the start value
        /// </summary>
        public void SetToStartValue()
        {
            Value = _startValue;
        }


        //Event

        // <summary>
        /// Raises the event with the generic value and notifies the listeners
        /// </summary>
        public void Raise(T value)
        {
            Value = value;
            Log(_stacktraceEvent, $"Raised and set value to {Value}");

            if (CheckForEventError()) return;

            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised(value);
                LogListener(_stacktraceEvent, $"Raised value: {value}", _listeners[i]);
            }
        }

        /// <summary>
        /// Raises the event with the Variable Value
        /// </summary>
        public void Raise()
        {
            Raise(Value);
        }

        /// <summary>
        /// Registers a listener for the event
        /// </summary>
        public void RegisterListener(IListener<T> listener)
        {
            if (CheckForEventError()) return;

            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
                LogListener(_stacktraceEvent, $"Registered listener", listener);
            }
            else
            {
                LogListener(_stacktraceEvent, $"Tried registering an existing listener", listener);
            }
        }

        /// <summary>
        /// Unregisters a listener for the event
        /// </summary>
        public void UnregisterListener(IListener<T> listener)
        {
            if (CheckForEventError()) return;

            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
                LogListener(_stacktraceEvent, $"Unregistered listener", listener);
            }
            else
            {
                LogListener(_stacktraceEvent, $"Tried removing a non existing listener", listener);
            }
        }

        /// <summary>
        /// Removes all listeners
        /// </summary>
        public void RemoveAllListeners()
        {
            _listeners.Clear();
            Log(_stacktraceEvent, "Cleared all listeners");
        }

        /// <summary>
        /// Checks if the variableType is not suited for events or if the listeners list is not inialized
        /// </summary>
        /// <returns></returns>
        private bool CheckForEventError()
        {
            if (!(VariableType == VariableType.VariableEvent || VariableType == VariableType.Event))
            {
                Debug.LogWarning($"Cant Raise event on a variable or RuntimeSet ({name})");
                return true;
            }

            if (_listeners == null)
            {
                Debug.LogWarning($"Listeners list is null ({name})");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the listeners as a IListener in a list
        /// </summary>
        public List<IListener> GetListeners() => _listeners.Cast<IListener>().ToList();

        public void InitializeListeners()
        {
            _listeners = new List<IListener<T>>();
        }

        //RuntimeSet

        /// <summary>
        /// Adds the given value to the runtimeset. Only possible when the variable type is not readonly
        /// </summary>
        public void Add(T value)
        {
            if (InitializeTypeRuntimeSet == InitializeType.ReadOnly)
            {
                Debug.LogWarning($"Cannot add {value} to: {name} (Readonly)");
                return;
            }

            if (VariableType != VariableType.RuntimeSet)
            {
                Debug.LogWarning("Cannot add the value to a non runtimeset");
                return;
            }

            if (_runtimeSet == null)
            {
                Debug.LogWarning("RuntimeSet not initialized");
                return;
            }

            _runtimeSet.Add(value);
            Log(_stacktraceRuntimeSet, $"Added value to runtimeSet: {value}");
        }

        /// <summary>
        /// Removes the given value from the runtimeset. Only possible when the variable type is not readonly
        /// </summary>
        public void Remove(T value)
        {
            if (InitializeTypeRuntimeSet == InitializeType.ReadOnly)
            {
                Debug.LogWarning($"Cannot remove {value} from: {name} (Readonly)");
                return;
            }

            _runtimeSet.Remove(value);
            Log(_stacktraceRuntimeSet, $"Removed value from runtimeSet: {value}");
        }

        /// <summary>
        /// Cleares the runtimeset. Only possible when the variable type is not readonly
        /// </summary>
        public void ClearRuntimeSet()
        {
            if (InitializeTypeRuntimeSet == InitializeType.ReadOnly)
            {
                Debug.LogWarning($"Cannot clear {name} (Readonly)");
                return;
            }

            _runtimeSet.Clear();
            Log(_stacktraceRuntimeSet, $"Cleared runtimeSet");
        }

        /// <summary>
        /// Initializes the runtimeset list
        /// </summary>
        public void InitializeRuntimeSet()
        {
            _runtimeSet = new List<T>();
        }


        //Debugging

        /// <summary>
        /// Logs a message to the stacktrace
        /// </summary>
        private void Log(Stacktrace stacktrace, string message)
        {
#if UNITY_EDITOR
            stacktrace.Add(message);
#endif
        }

        /// <summary>
        /// Loga a message to the stacktrace with a listener. Appends the listener name if not null
        /// </summary>
        private void LogListener(Stacktrace stacktrace, string message, IListener listener)
        {
#if UNITY_EDITOR
            stacktrace.Add($"{message}" + (listener != null ? $" ({listener.GetName()})" : ""));
#endif
        }

        /// <summary>
        /// Gets all stacktraces (Variable, Event, Runtimeset)
        /// </summary>
        public Stacktrace[] GetStackTraces() => new Stacktrace[] { _stacktraceVariable, _stacktraceEvent, _stacktraceRuntimeSet };

        //Initialize stack traces for each variable type with a maximum capacity
        private Stacktrace _stacktraceVariable = new Stacktrace(VariableType.Variable, 14);
        private Stacktrace _stacktraceEvent = new Stacktrace(VariableType.Event, 100);
        private Stacktrace _stacktraceRuntimeSet = new Stacktrace(VariableType.RuntimeSet, 100);

        public T DebugValue;
    }

    public abstract class KartStatsVariable : ScriptableObject
    {
        public abstract IStats GetStats();

        public VariableType VariableType;
        public InitializeType InitializeTypeVariable;
        public InitializeType InitializeTypeRuntimeSet;
    }
}