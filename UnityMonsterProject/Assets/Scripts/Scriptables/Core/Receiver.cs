using System.Collections.Generic;
using UnityEngine;

namespace ScriptableArchitecture.Core
{
    /// <summary>
    /// ScriptableObject responsible for managing emitters and determining the active emitter based on priority
    /// </summary>
    [CreateAssetMenu(fileName = "Receiver", menuName = "Scriptables/Receiver")]
    public class Receiver : ScriptableObject
    {
        [SerializeField] private List<Emitter> _emitters = new List<Emitter>();
        [SerializeField] private Emitter _activeEmitter;

        /// <summary>
        /// Gets the value from the currently active emitter of the specified type
        /// </summary>
        public T Value<T>() where T : Object
        {
            if (_activeEmitter == null)
            {
                Debug.LogError($"No active emitter in {name}, make sure to first add an emitter");
                Debug.Break();
                return null;
            }

            return _activeEmitter.Value<T>();
        }

        /// <summary>
        /// Registers an emitter on this receiver
        /// </summary>
        public void RegisterEmitter(Emitter emitter)
        {
            if (_emitters.Contains(emitter)) return;

            _emitters.Add(emitter);
            UpdatePriorityList(emitter);
        }

        /// <summary>
        /// Unregisters an emitter on this receiver
        /// </summary>
        public void UnregisterEmitter(Emitter emitter)
        {
            if (!_emitters.Contains(emitter)) return;

            _emitters.Remove(emitter);

            if (_activeEmitter == emitter)
                UpdatePriorityList();
        }

        /// <summary>
        /// Removes all emitters from this receiver
        /// </summary>
        public void RemoveAllEmitters()
        {
            _emitters.Clear();
        }

        /// <summary>
        /// Updates the priority list based on a changed emitter
        /// </summary>
        public void UpdatePriorityList(Emitter changedEmitter)
        {
            if (_activeEmitter == null || changedEmitter == _activeEmitter || changedEmitter.Priority > _activeEmitter.Priority)
                UpdatePriorityList();
        }

        /// <summary>
        /// Updates the priority list of the active emitter. The emmiter with the highest priority is set to the active emmiter
        /// </summary>
        public void UpdatePriorityList()
        {
            if (_emitters == null || _emitters.Count == 0 || _emitters[0] == null) 
            {
                _activeEmitter = null;
                return;
            }
            
            Emitter priorityEmitter = _emitters[0];
            int priority = priorityEmitter.Priority;

            for(int i = 1; i < _emitters.Count; i++)
            {
                if (_emitters[i] == null)
                    continue;

                if (_emitters[i].Priority > priority)
                {
                    priorityEmitter = _emitters[i];
                    priority = priorityEmitter.Priority;
                }
            }

            _activeEmitter = priorityEmitter;
        }
    }
}