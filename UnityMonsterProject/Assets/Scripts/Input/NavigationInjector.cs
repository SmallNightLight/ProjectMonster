using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationInjector : MonoBehaviour
{
    [SerializeField] private List<InputAssetReference> _inputDatas;

    [SerializeField] private GameEvent _backEvent;

    private bool _combineInput = true;

    private void Update()
    {
        InputData UIInput = new InputData();

        //Allow every player to navigate UI
        foreach (InputAssetReference inputData in _inputDatas)
        {
            UIInput.Press |= inputData.Value.InputData.Press;
            UIInput.Back |= inputData.Value.InputData.Back;
            UIInput.MoveUp |= inputData.Value.InputData.MoveUp;
            UIInput.MoveDown |= inputData.Value.InputData.MoveDown;
            UIInput.MoveRight |= inputData.Value.InputData.MoveRight;
            UIInput.MoveLeft |= inputData.Value.InputData.MoveLeft;
        }

        MoveDirection moveDirection = GetMoveDirection(UIInput.MoveUp, UIInput.MoveDown, UIInput.MoveRight, UIInput.MoveLeft);

        if (moveDirection != MoveDirection.None)
            Move(moveDirection);

        if (UIInput.Press)
            Press();

        if (UIInput.Back)
            _backEvent.Raise();
    }

    public void Move(MoveDirection direction)
    {
        AxisEventData data = new AxisEventData(EventSystem.current);

        data.moveDir = direction;

        data.selectedObject = EventSystem.current.currentSelectedGameObject;

        ExecuteEvents.Execute(data.selectedObject, data, ExecuteEvents.moveHandler);
    }

    public void Press()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        if (selectedObject != null)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(selectedObject, pointerEventData, ExecuteEvents.submitHandler);
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

    public void CombineInput(bool state)
    {
        _combineInput = state;
    }
}