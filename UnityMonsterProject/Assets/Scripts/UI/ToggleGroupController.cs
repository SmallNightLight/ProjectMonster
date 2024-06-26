using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleGroupController : MonoBehaviour
{
    [SerializeField] private InputAssetReference _input;
    [SerializeField] private Toggle initialToggle; // Reference to the initial toggle
    public ToggleGroup toggleGroup;

    [SerializeField] private List<Toggle> toggles;
    private Toggle currentSelectedToggle;

    int currentIndex;

    [SerializeField] private UnityEvent _back;

    private void Start()
    {
        if (toggleGroup == null) return;

        if (initialToggle != null && toggles.Contains(initialToggle))
        {
            currentSelectedToggle = initialToggle;
        }
        else if (toggles.Count > 0)
        {
            currentSelectedToggle = toggles[0];
        }

        if (currentSelectedToggle != null)
        {
            SelectObject(currentSelectedToggle);
            currentIndex = toggles.IndexOf(currentSelectedToggle);
        }
    }

    private void Update()
    {
        MoveDirection moveDirection = GetMoveDirection(
            _input.Value.InputData.MoveUp,
            _input.Value.InputData.MoveDown,
            _input.Value.InputData.MoveRight,
            _input.Value.InputData.MoveLeft
        );

        if (moveDirection != MoveDirection.None)
            Move(moveDirection);

        if (_input.Value.InputData.Press)
        {
            PressedObject(toggles[currentIndex]);
        }        
        
        if (_input.Value.InputData.Back)
        {
            _back.Invoke();
        }
    }

    public void SelectObject(Toggle target)
    {
        if (target == null) return;

        target.GetComponent<Animator>().SetTrigger("Selected");
        OtherNormal(target);
        target.GetComponent<SetCharacterData>().Select();
    }

    public void PressedObject(Toggle target)
    {
        if (target == null) return;

        target.GetComponent<Animator>().SetTrigger("Pressed");
        OtherNormal(target);
        target.GetComponent<SetCharacterData>().Press();
    }

    public void OtherNormal(Toggle exclude)
    {
        foreach (Toggle toggle in toggles)
        {
            if (toggle != exclude)
                toggle.GetComponent<Animator>().SetTrigger("Normal");
        }
    }

    public void Move(MoveDirection direction)
    {
        if (currentSelectedToggle != null && toggles.Contains(currentSelectedToggle))
        {
            switch (direction)
            {
                case MoveDirection.Left:
                    currentIndex = (currentIndex > 0) ? currentIndex - 1 : toggles.Count - 1;
                    break;
                case MoveDirection.Right:
                    currentIndex = (currentIndex < toggles.Count - 1) ? currentIndex + 1 : 0;
                    break;
                default:
                    return;
            }

            SelectObject(toggles[currentIndex]);
        }
    }

    private MoveDirection GetMoveDirection(bool up, bool down, bool right, bool left)
    {
        if (up) return MoveDirection.Up;
        if (down) return MoveDirection.Down;
        if (right) return MoveDirection.Right;
        if (left) return MoveDirection.Left;

        return MoveDirection.None;
    }
}