using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationInjector : MonoBehaviour
{
    [SerializeField] private List<InputAssetReference> _inputDatas;

    [SerializeField] private GameEvent _backEvent;

    public bool _selectObjects = true;

    private void Update()
    {
        if (_selectObjects)
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
    }   

    public void Press()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
       
        if (selectedObject != null)
            ClickObject(selectedObject);
    }

    public void SetSetectObjects(bool value)
    {
        _selectObjects = value;
    }

    public void ClickObject(GameObject target)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(target, pointerEventData, ExecuteEvents.submitHandler);
    }

    public void Move(MoveDirection direction)
    {
        AxisEventData data = new AxisEventData(EventSystem.current);

        data.moveDir = direction;

        data.selectedObject = EventSystem.current.currentSelectedGameObject;

        ExecuteEvents.Execute(data.selectedObject, data, ExecuteEvents.moveHandler);
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