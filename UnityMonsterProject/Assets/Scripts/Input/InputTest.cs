using ScriptableArchitecture.Data;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    //[SerializeField] private 
    [SerializeField] private TextAsset _layout;
    private Gamepad controller = null;
    private Transform m_transform;
    private DS4 ds4;

    void Start()
    {
        if (ds4 != null) return;

        ds4 = new DS4();
        this.controller = ds4.getConroller(_layout.text);
        m_transform = this.transform;
    }

    public void Setup(int i = 0)
    {
        ds4 = new DS4();
        this.controller = ds4.getConroller(_layout.text, padIndex: i);
        m_transform = this.transform;
    }

    void Update()
    {
        if (controller == null)
        {
            try
            {
                controller = ds4.getConroller(_layout.text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        else
        {
            // Press circle button to reset rotation
            if (controller.rightShoulder.isPressed)
            {
                ResetC();
            }
            m_transform.rotation *= ds4.getRotation(_scale * Time.deltaTime);
        }

        data = GetData();
    }

    [SerializeField] private float _scale = 10000;

    public void StartGameCalibrate(GameData data)
    {
        if (data.State == GameState.Gameplay)
        {
            ResetC();
        }
    }

    public void ResetC()
    {
        m_transform.rotation = Quaternion.identity;
    }

    [SerializeField] InputData data;
    [SerializeField] private float _inputF = 0.5f;

    public InputData GetData()
    {
        float yaw = m_transform.rotation.eulerAngles.y;
        float steer = Mathf.InverseLerp(-90, 90, Mathf.DeltaAngle(0, yaw)) * 2 - 1;

        return new InputData
        {
            IsAccelerating = controller.buttonEast.isPressed,
            IsBraking = controller.buttonSouth.isPressed,
            SteerInput = controller.leftStick.ReadValue().x * _inputF,
            //SteerInput = steer,
            IsTricking = false,
            AbilityBoost = controller.leftTrigger.isPressed,
            Ability1 = controller.rightTrigger.isPressed,

            Press = controller.buttonEast.wasPressedThisFrame,
            Back = controller.buttonSouth.isPressed,

            MoveUp = controller.dpad.up.wasPressedThisFrame,
            MoveDown = controller.dpad.down.wasPressedThisFrame,
            MoveRight = controller.dpad.right.wasPressedThisFrame,
            MoveLeft = controller.dpad.left.wasPressedThisFrame,
            IsPS = true
        };
    }

    public Gamepad Pad => controller;
}