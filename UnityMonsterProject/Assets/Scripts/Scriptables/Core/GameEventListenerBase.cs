using UnityEngine;
using UnityEngine.Events;

namespace ScriptableArchitecture.Core
{
    /// <summary>
    /// Base class for empty game event listeners
    /// </summary>
    public abstract class GameEventListenerBase : MonoBehaviour, IListenerEmpty
    {
        [SerializeField] private GameEventBase _event;
        [SerializeField] private UnityEvent _response;
        
        /// <summary>
        /// Invokes the unity event when the event is raised
        /// </summary>
        public void OnEventRaised()
        {
            _response.Invoke();
        }

        /// <summary>
        /// Registeres the listener to the event on enable
        /// </summary>
        private void OnEnable()
        {
            if (_event == null)
            {
                Debug.LogWarning($"No Event selected on {name}");
                return;
            }
                
            _event.RegisterListener(this);
        }

        /// <summary>
        /// Unregisteres the listener to the event on disable
        /// </summary>
        private void OnDisable()
        {
            if (_event == null) return;

            _event.UnregisterListener(this);
        }

        /// <summary>
        /// Gets the listener as an object
        /// </summary>
        public Object GetListenerObject() => this;

        /// <summary>
        /// Gets the attached game event
        /// </summary>
        public IGameEvent GetGameEvent() => _event;

        /// <summary>
        /// Gets the name of the listener
        /// </summary>
        public string GetName() => name;
    }

    /// <summary>
    /// Base class for generic game event listeners
    /// </summary>
    public abstract class GameEventListenerBase<T> : MonoBehaviour, IListener<T>
    {
        [SerializeField] private UnityEvent<T> _response;

        /// <summary>
        /// Invokes the unity event when the event is raised
        /// </summary>
        public void OnEventRaised(T value)
        {
            _response.Invoke(value);
        }

        /// <summary>
        /// Registeres the listener to the event on enable
        /// </summary>
        private void OnEnable()
        {
            IGameEvent<T> genericEvent = GetGameEventT();

            if (genericEvent == null)
            {
                Debug.LogWarning($"No Event selected on {name}");
                return;
            }

            genericEvent.RegisterListener(this);
        }


        /// <summary>
        /// Unregisteres the listener to the event on disable
        /// </summary>
        private void OnDisable()
        {
            IGameEvent<T> genericEvent = GetGameEventT();

            if (genericEvent == null)
                return;

            genericEvent.UnregisterListener(this);
        }

        /// <summary>
        /// Gets the listener as an object
        /// </summary>
        public Object GetListenerObject() => this;

        /// <summary>
        /// Ovveride this function and get the game event with the correct generic
        /// </summary>
        public abstract IGameEvent<T> GetGameEventT();

        /// <summary>
        /// Gets the attached game event
        /// </summary>
        public IGameEvent GetGameEvent() => GetGameEventT();

        /// <summary>
        /// Gets the name of the listener
        /// </summary>
        public string GetName() => name;
    }

    /// <summary>
    /// Interface for a base game event listener
    /// </summary>
    public interface IListener 
    {
        /// <summary>
        /// Gets the listener as an object
        /// </summary>
        public Object GetListenerObject();

        /// <summary>
        /// Gets the game event attached to the listener
        /// </summary>
        public IGameEvent GetGameEvent();

        /// <summary>
        /// Gets the name of the listener
        /// </summary>
        public string GetName();
    }

    /// <summary>
    /// Interface for ane empty game event listener
    /// </summary>
    public interface IListenerEmpty : IListener
    {
        /// <summary>
        /// Method called when the event is raised
        /// </summary>
        public void OnEventRaised();
    }

    /// <summary>
    /// Interface for a game event listener with a generic type.
    /// </summary>
    public interface IListener<T> : IListener
    {
        /// <summary>
        /// Method called when the event with is raised with a value
        /// </summary>
        public void OnEventRaised(T value);

        /// <summary>
        /// Gets the generic game event attached to the listener
        /// </summary>
        public IGameEvent<T> GetGameEventT();
    }
}