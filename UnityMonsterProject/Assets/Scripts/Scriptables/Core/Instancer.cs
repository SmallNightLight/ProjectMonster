using UnityEngine;

namespace ScriptableArchitecture.Core
{
    [DefaultExecutionOrder(-2000)]
    public class Instancer<TVariable> : ReferenceInstancer where TVariable : Variable
    {
        [SerializeField] protected InstanceScope _instanceScope;
        [SerializeField] protected TVariable _baseVariable;
        [SerializeField] protected TVariable _instancedVariable;

        private void Awake()
        {
            InitializeInstance();
        }

        public void InitializeInstance()
        {
            if (_baseVariable == null) return;

            if (_instanceScope == null)
            {
                _instancedVariable = Instantiate(_baseVariable);
            }
            else
            {
                _instancedVariable = _instanceScope.GetOrInstanceVariable(_baseVariable) as TVariable;
            }
        }

        public TVariable Instance
        {
            get
            {
                return _instancedVariable;
            }
            set
            {
                _instancedVariable = value;
            }
        }
    }

    public class ReferenceInstancer : MonoBehaviour { }
}