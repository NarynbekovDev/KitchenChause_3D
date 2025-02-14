using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public const string PLAYER_PREFS_BINDING = "InputBindings";

    public static GameInput Instance { get; private set; }


    public event EventHandler OnInteractAction;
    public event EventHandler onInteractAlternateAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;


    public enum Binding 
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause
    }


    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        Instance = this;

        _playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDING))
        {
            _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDING));
        }

        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Interact.performed += Interact_performed;
        _playerInputActions.Player.InteractAlternet.performed += InteractAlternet_performed;
        _playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Interact.performed -= Interact_performed;
        _playerInputActions.Player.InteractAlternet.performed -= InteractAlternet_performed;
        _playerInputActions.Player.Pause.performed -= Pause_performed;

        _playerInputActions.Dispose();
    }


    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternet_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        onInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        #region This code is longer than the lower code 
        //if (OnInteractAction != null)
        //{
        //    OnInteractAction(this, EventArgs.Empty);
        //}
        #endregion  
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public  Vector2 GetMoveMentVectorNormalized()
    {
        #region This is Scripts for Walk, but it's other scripts!
        //Vector2 inputVector = new Vector2(0, 0);
        //if (Input.GetKey(KeyCode.S))
        //{
        //    inputVector.y = -1;
        //}
        //if (Input.GetKey(KeyCode.W))
        //{
        //    inputVector.y = 1;
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    inputVector.x = -1;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    inputVector.x = 1;
        //}
        #endregion

        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding) 
        {
            default:
            case Binding.Move_Up:
                return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return _playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return _playerInputActions.Player.InteractAlternet.bindings[0].ToDisplayString();
            case Binding.Pause:
                return _playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return _playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_InteractAlternate:
                return _playerInputActions.Player.InteractAlternet.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return _playerInputActions.Player.Pause.bindings[1].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        _playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;   

        switch (binding) 
        {
            default:
            case Binding.Move_Up:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = _playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = _playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = _playerInputActions.Player.InteractAlternet;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Gamepad_Interact:
                inputAction = _playerInputActions.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_InteractAlternate:
                inputAction = _playerInputActions.Player.InteractAlternet;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                inputAction = _playerInputActions.Player.Pause;
                bindingIndex = 1;
                break;
        }


        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
        {
            callback.Dispose();
            _playerInputActions.Player.Enable();
            onActionRebound();

            PlayerPrefs.SetString(PLAYER_PREFS_BINDING, _playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();

            OnBindingRebind?.Invoke(this, EventArgs.Empty);

        }).Start();
    }
}
