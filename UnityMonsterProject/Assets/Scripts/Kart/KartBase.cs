using ScriptableArchitecture.Data;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-15)]
public class KartBase : MonoBehaviour
{
    [SerializeField] private InputAssetReference _input;

    //private PlayerInput _inputActions;
    private InputData _inputData;

    //private InputAction _steeringInput;

    //public delegate void Ability1Input();
    //public Ability1Input Ability1;

    //private void Awake()
    //{
    //    _inputactions = new playerinput();
    //    _inputData = new InputData();
    //}

    //private void OnEnable()
    //{
    //    _steeringInput = _inputActions.Player.Steering;

    //    _inputActions.Player.Accelerate.started += (InputAction.CallbackContext obj) => { _inputData.IsAccelerating = true; };
    //    _inputActions.Player.Accelerate.canceled += (InputAction.CallbackContext obj) => { _inputData.IsAccelerating = false; };
    //    _inputActions.Player.Accelerate.Enable();

    //    _inputActions.Player.Break.started += (InputAction.CallbackContext obj) => { _inputData.IsBraking = true; };
    //    _inputActions.Player.Break.canceled += (InputAction.CallbackContext obj) => { _inputData.IsBraking = false; };
    //    _inputActions.Player.Break.Enable();

    //    _steeringInput.Enable();

    //    _inputActions.Player.Trick.performed += (InputAction.CallbackContext obj) => { _inputData.IsTricking = true; };
    //    _inputActions.Player.Trick.canceled += (InputAction.CallbackContext obj) => { _inputData.IsTricking = false; };
    //    _inputActions.Player.Trick.Enable();

    //    _inputActions.Player.Ability1.performed += (InputAction.CallbackContext obj) => { Ability1(); };
    //    _inputActions.Player.Ability1.Enable();
    //}

    //private void Update()
    //{
    //    _inputData.SteerInput = _steeringInput.ReadValue<float>();
    //}

    //private void OnDisable()
    //{
    //    _inputActions.Player.Accelerate.Disable();
    //    _inputActions.Player.Break.Disable();
    //    _inputActions.Player.Steering.Disable();
    //    _inputActions.Player.Trick.Disable();
    //    _inputActions.Player.Ability1.Disable();
    //}

    public InputData Input => _input.Value.InputData;
    public int Player => _input.Value.Player;
}