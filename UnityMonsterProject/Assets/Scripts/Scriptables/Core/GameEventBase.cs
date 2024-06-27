using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableArchitecture.Core
{
    /// <summary>
    /// Empty Game event scriptable object that can be raised and configured with listeners
    /// </summary>
    public class GameEventBase : ScriptableObject, IGameEvent
    {
        private List<IListenerEmpty> _listeners = new List<IListenerEmpty>();

        /// <summary>
        /// Raises the event and notifies all registered listeners
        /// </summary>
        public void Raise()
        {
            Log("Raised", null);
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
                Log($"Raised", _listeners[i]);
            }
        }

        /// <summary>
        /// Registers a listener for the event
        /// </summary>
        public void RegisterListener(IListenerEmpty listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
                Log($"Registered listener", listener);
            }
            else
            {
                Log($"Tried registering an existing listener", listener);
            }
        }

        /// <summary>
        /// Unregisters a listener from the event
        /// </summary>
        public void UnregisterListener(IListenerEmpty listener)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
                Log($"Unregistered listener", listener);
            }
            else
            {
                Log($"Tried removing a non existing listener", listener);
            }   
        }

        /// <summary>
        /// Removes all listeners from the event
        /// </summary>
        public void RemoveAllListeners()
        {
            _listeners.Clear();
            Log("Cleared all listeners", null);
        }

        /// <summary>
        /// Gets all listeners registered for the event
        /// </summary>
        public List<IListener> GetListeners() => _listeners.Cast<IListener>().ToList();


        //Stack trace

        /// <summary>
        /// Clears the stacktrace on enable
        /// </summary>
        private void OnEnable()
        {
            _stacktrace.Clear();
        }

        /// <summary>
        /// Logs a message to the stacktrace and adds the name of the listeners to the end. Set the listeners to null for no listener data
        /// </summary>
        private void Log(string message, IListener listener)
        {
#if UNITY_EDITOR
            _stacktrace.Add($"{message}" + (listener != null ? $" ({listener.GetName()})" : ""));
#endif
        }

        /// <summary>
        /// Get the stacktrace
        /// </summary>
        public Stacktrace[] GetStackTraces() => new Stacktrace[] { _stacktrace };
        private Stacktrace _stacktrace = new Stacktrace(VariableType.Event, 100);
    }

    /// <summary>
    /// Interface for empty game events
    /// </summary>
    public interface IGameEvent
    {
        /// <summary>
        /// Gets all listeners registered for the event
        /// </summary>
        public List<IListener> GetListeners();

        /// <summary>
        /// Gets the stack traces associated with the event
        /// </summary>
        public Stacktrace[] GetStackTraces();
    }

    /// <summary>
    /// Generic interface for game events
    /// </summary>
    public interface IGameEvent<T> : IGameEvent
    {
        /// <summary>
        /// Registers a listener for the event
        /// </summary>
        public void RegisterListener(IListener<T> listener);

        /// <summary>
        /// Unregisters a listener from the event
        /// </summary>
        public void UnregisterListener(IListener<T> listener);
    }
}