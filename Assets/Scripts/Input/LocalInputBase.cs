using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public abstract class LocalInputBase : MonoBehaviour
{
    protected PlayerInputActions _inputActions;
    protected InputActionMapType _currentActionMap;
    protected InputDevice _currentInputDevice;
    protected InputUser _inputUser;

    private IDisposable _onAnyButtonPressSubscription;
    public InputDevice CurrentInputDevice => _currentInputDevice;

    private const int MaxUnpairedDeviceCount = 10;
    protected virtual void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        SubscribeToInputEvents();

        if (Gamepad.current != null) // prioritize gamepad
        {
            _currentInputDevice = Gamepad.current;
        }
        else if (InputSystem.devices.Count > 0)
        {
            _currentInputDevice = InputSystem.devices[0];
        }
        if (_currentInputDevice != null)
        {
            _inputUser = InputUser.PerformPairingWithDevice(_currentInputDevice);
            _inputUser.AssociateActionsWithUser(_inputActions);
        }

        InputUser.listenForUnpairedDeviceActivity = MaxUnpairedDeviceCount;

        InputSystem.onDeviceChange += OnDeviceChange;
        _onAnyButtonPressSubscription = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
    }

    protected virtual void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        _onAnyButtonPressSubscription?.Dispose();
    }

    protected virtual void SubscribeToInputEvents() { }

    #region Input Action Map Management

    protected void SwitchToInputActionMap(InputActionMapType type)
    {
        _inputActions.Gameplay.Disable();
        _inputActions.Inventory.Disable();
        _inputActions.PausedGamplay.Disable();
        _inputActions.GameOverScreen.Disable();
        _inputActions.Menu.Disable();

        var targetMap = ActionMapTypeToActionMap(type);
        targetMap.Enable();

        _inputActions.UI.Enable();

        _currentActionMap = type;
        RebindToCurrentDevice();
    }

    protected InputActionMap ActionMapTypeToActionMap(InputActionMapType type)
    {
        return type switch
        {
            InputActionMapType.Gameplay => _inputActions.Gameplay,
            InputActionMapType.Inventory => _inputActions.Inventory,
            InputActionMapType.PausedGameplay => _inputActions.PausedGamplay,
            InputActionMapType.GameOverScreen => _inputActions.GameOverScreen,
            InputActionMapType.Menu => _inputActions.Menu,
            _ => throw new Exception("Invalid input action type: " + type),
        };
    }

    #endregion

    private void OnAnyButtonPress(InputControl control)
    {
        var device = control.device;

        if (device == _currentInputDevice)
            return;

        _currentInputDevice = device;
        RebindToCurrentDevice();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
        {
            _currentInputDevice = device;
            RebindToCurrentDevice();
        }
    }

    private void RebindToCurrentDevice()
    {
        if (!_currentInputDevice.added)
            return;

        if (_currentInputDevice is Keyboard || _currentInputDevice is Mouse)
        {
            Cursor.visible = true;
        }

        if (_inputUser.valid)
        {
            _inputUser.UnpairDevicesAndRemoveUser();
        }

        _inputUser = InputUser.PerformPairingWithDevice(_currentInputDevice);
        _inputUser.AssociateActionsWithUser(_inputActions);

        Debug.Log($"Input paired with {_currentInputDevice.displayName}");
    }
}
