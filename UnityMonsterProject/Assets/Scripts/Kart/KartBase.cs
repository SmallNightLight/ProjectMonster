using ScriptableArchitecture.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-15)]
public class KartBase : MonoBehaviour
{
    [SerializeField] protected InputAssetReference _input;

    [Header("Components")]
    [SerializeField] private GameObject _kartVisualParent;
    [SerializeField] private GameObject _characterVisualParent;

    public RoadSplines Splines;
    public CharacterData CharacterData;

    private bool _isActive;

    public bool IsActive 
    {
        get
        {
            return _isActive;
        }
        set
        {
            _isActive = value;

            if (IsActive)
                StartCoroutine(WaitForUpdateTarget());
            else
                StopCoroutine(WaitForUpdateTarget());
        }
    }

    [SerializeField] private PlacementReference _placements;
    [HideInInspector] public Vector3 SplinesTargetPosition { get; private set; }
    [HideInInspector] public int SplinesSpline;
    [HideInInspector] public float SplinesStep;
    public float SplinesPercentage = 0.5f;

    private KartLabs _kartLabs;

    //private PlayerInput _inputActions;
    //private InputData _inputData;

    //private InputAction _steeringInput;

    //public delegate void Ability1Input();
    //public Ability1Input Ability1;

    public virtual void Start()
    {
        UpdateVisuals();
        TryGetComponent(out _kartLabs);
        
        SplinesSpline = 0;
        SplinesStep = 0;

        UpdateTarget();
    }

    [ContextMenu("Update Visuals")]
    public void UpdateVisuals()
    {
        if (_kartVisualParent)
        {
            foreach (Transform child in _kartVisualParent.transform)
            {
                Destroy(child.gameObject);
            }

            if (CharacterData.KartPrefab)
                Instantiate(CharacterData.KartPrefab, _kartVisualParent.transform);
        }

        if (_characterVisualParent)
        {
            foreach (Transform child in _characterVisualParent.transform)
            {
                Destroy(child.gameObject);
            }

            if (CharacterData.CharacterPrefab)
                Instantiate(CharacterData.CharacterPrefab, _characterVisualParent.transform);
        }
    }

    public void ChangeGameState(GameData gameData)
    {
        switch (gameData.State)
        {
            case GameState.StartCinematic:
                IsActive = false;
                break;
            case GameState.CountDown:
                IsActive = false;
                break;
            case GameState.Gameplay:
                IsActive = true;
                break;
            case GameState.EndCinematic:
                IsActive = false;
                break;
        }
    }

    public void DisablePlayer()
    {
        IsActive = false;
    }

    private IEnumerator WaitForUpdateTarget()
    {
        UpdateTarget();

        yield return new WaitForSeconds(Random.Range(0f, 0.2f));

        while (true)
        {
            if (IsActive)
                UpdateTarget();

            yield return null;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void UpdateTarget(bool reachedFinish = false)
    {
        Splines.GetNextSidePositions(transform.position, ref SplinesSpline, ref SplinesStep, out Vector3 side1, out Vector3 side2);
        SplinesTargetPosition = Vector3.Lerp(side1, side2, SplinesPercentage);

        int currentLab = 1;
        if (_kartLabs)
            currentLab = _kartLabs.GetCurrentLab();

        Debug.Log(Player);
        _placements.Value.UpdatePlayer(Player, currentLab, SplinesSpline, SplinesStep, reachedFinish);
    }

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

    public GameObject KartVisualParent => _kartVisualParent;
    public GameObject CharacterVisualParent => _characterVisualParent;
}