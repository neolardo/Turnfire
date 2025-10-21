using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputManager : MonoBehaviour
{

    [SerializeField] private AnimationCurve gamepadStickResponseCurve =
    new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 0.2f),
        new Keyframe(1, 1));

    private float _mouseAimRadius;
    private Vector2 _aimVector;
    private Vector2 _initialMouseAimPosition;
    private bool _isAiming;
    private bool _isInitialMouseAimPositionSet;
    private PlayerInputActions _inputActions;

    private InputActionMapType _currentActionMap;

    public InputDevice CurrentInputDevice => _currentInputDevice;
    private InputDevice _currentInputDevice;

    public bool IsAimingEnabled { get; set; }
    public bool IsOpeningInventoryEnabled { get; set; }
    public bool IsPausingGameplayEnabled { get; set; }

    //gameplay
    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;
    //inventory
    public event Action ToggleInventoryPerformed;
    public event Action ToggleInventoryCreateDestroyPerformed;
    public event Action TogglePauseGameplayPerformed;
    public event Action SelectInventorySlotPerformed;
    //menu
    public event Action MenuConfirmPerformed;
    public event Action MenuBackPerformed;


    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        IsPausingGameplayEnabled = true;
        SwitchToInputActionMap(InputActionMapType.Gameplay);
        SubscribeToInputEvents();
    }

    private void SubscribeToInputEvents()
    {
        _inputActions.Gameplay.Aim.performed += OnAimPerformed;
        _inputActions.Gameplay.Aim.canceled += OnAimCancelled;
        _inputActions.Gameplay.ReleaseImpulse.started += OnImpulseReleaseStarted;
        _inputActions.Gameplay.ReleaseImpulse.canceled += OnImpulseReleaseEnded;
        _inputActions.Gameplay.Cancel.started += OnCancelPerformed;
        _inputActions.Gameplay.ToggleInventory.started += OnToggleInventory;
        _inputActions.Gameplay.PauseGameplay.started += OnTogglePauseGameplay;
        _inputActions.PausedGamplay.ResumeGameplay.started += OnTogglePauseGameplay;
        _inputActions.Inventory.ToggleInventory.started += OnToggleInventory;
        _inputActions.Inventory.ToggleCreateDestroy.started += OnToggleCreateDestroy;
        _inputActions.Inventory.ToggleCreateDestroy.started += OnToggleCreateDestroy;
        _inputActions.Menu.Back.performed += OnMenuBackPerformed;
        _inputActions.Menu.Confirm.performed += OnMenuConfirmPerformed;
        InputSystem.onDeviceChange += OnDeviceChange;
        InputSystem.onAnyButtonPress.CallOnce(control => OnDeviceChange(control.device, InputDeviceChange.Reconnected));
    }

    #region Gameplay

    private void OnAimPerformed(InputAction.CallbackContext ctx)
    {
        if (!IsAimingEnabled)
            return;

        if (ctx.control.device is Gamepad)
        {
            HandleGamepadAiming(ctx);
        }
        else if (ctx.control.device is Mouse)
        {
            HandleMouseAiming(ctx);
        } //TODO: other inputs?
    }

    private void HandleGamepadAiming(InputAction.CallbackContext ctx)
    {
        _aimVector = GetGamepadStickValue(ctx);
        if (!_isAiming)
        {
            AimStarted?.Invoke(new Vector2(-1, -1)); //TODO: no magic
        }
        _isAiming = true;
        SanitizeAimVector();
        AimChanged?.Invoke(_aimVector);
    }

    private void HandleMouseAiming(InputAction.CallbackContext ctx)
    {
        if (_isAiming)
        {
            var pos = ctx.ReadValue<Vector2>();
            if (!_isInitialMouseAimPositionSet)
            {
                _isInitialMouseAimPositionSet = true;
                _initialMouseAimPosition = pos;
            }
            _aimVector = _initialMouseAimPosition - pos;
            if (_aimVector.magnitude > _mouseAimRadius)
            {
                _aimVector = _aimVector.normalized * _mouseAimRadius;
                Mouse.current.WarpCursorPosition(_initialMouseAimPosition - _aimVector);
            }
            _aimVector = _aimVector / _mouseAimRadius;
            SanitizeAimVector();
            AimChanged?.Invoke(_aimVector);
        }
    }

    private void SanitizeAimVector()
    {
        if (float.IsNaN(_aimVector.x) || float.IsNaN(_aimVector.y))
        {
            Debug.Log("Aim vector was NaN.");
            _aimVector = new Vector2(0, 0);
        }
    }


    private void OnAimCancelled(InputAction.CallbackContext ctx)
    {
        if (!IsAimingEnabled)
            return;

        _aimVector = Vector2.zero;
        if (_isAiming)
        {
            AimChanged?.Invoke(_aimVector);
            _isInitialMouseAimPositionSet = false;
        }
    }

    private void OnImpulseReleaseStarted(InputAction.CallbackContext ctx)
    {
        if (!IsAimingEnabled)
            return;

        _isAiming = true;
        _isInitialMouseAimPositionSet = false;
        var initialPos = new Vector2(-1, -1);
        if (ctx.control.device is Mouse)
        {
            var mouse = (ctx.control.device as Mouse);
            initialPos = new Vector2(mouse.position.x.magnitude, mouse.position.y.magnitude);
            _mouseAimRadius = (Constants.AimCircleOuterRadiusPercent - Constants.AimCircleInnerRadiusPercent) * Screen.width;
        }
        AimStarted?.Invoke(initialPos);
        Cursor.visible = false;
    }

    private void OnImpulseReleaseEnded(InputAction.CallbackContext ctx)
    {
        if (!IsAimingEnabled || !_isAiming)
            return;

        _isAiming = false;
        ImpulseReleased?.Invoke(_aimVector);
        Cursor.visible = true;
    }

    private Vector2 GetGamepadStickValue(InputAction.CallbackContext ctx)
    {
        Vector2 raw = ctx.ReadValue<Vector2>();
        float mag = raw.magnitude;
        Vector2 normalized = raw.normalized;
        float adjustedMag = gamepadStickResponseCurve.Evaluate(mag);
        return -normalized * adjustedMag;
    }

    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        CancelAiming();
    }

    private void CancelAiming()
    {
        if (_isAiming)
        {
            _isAiming = false;
            Cursor.visible = true;
            AimCancelled?.Invoke();
        }
    }

    #endregion

    #region Inventory
    private void OnSelectInventorySlot(InputAction.CallbackContext ctx)
    {
        SelectInventorySlotPerformed?.Invoke();
    }

    private void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        var targetActionMapType = _currentActionMap == InputActionMapType.Gameplay ? InputActionMapType.Inventory : InputActionMapType.Gameplay;
        if (!IsOpeningInventoryEnabled && targetActionMapType == InputActionMapType.Inventory)
        {
            return;
        }
        CancelAiming();
        SwitchToInputActionMap(targetActionMapType);
        ToggleInventoryPerformed?.Invoke();
    }

    private void ForceCloseInventory()
    {
        if (_currentActionMap == InputActionMapType.Inventory)
        {
            ToggleInventoryPerformed?.Invoke();
        }
    }

    private void OnToggleCreateDestroy(InputAction.CallbackContext ctx)
    {
        ToggleInventoryCreateDestroyPerformed?.Invoke();
    }

    #endregion

    #region Menu

    private void OnMenuBackPerformed(InputAction.CallbackContext ctx)
    {
        MenuBackPerformed?.Invoke();
    }
    private void OnMenuConfirmPerformed(InputAction.CallbackContext ctx)
    {
        MenuConfirmPerformed?.Invoke();
    }

    #endregion

    #region Input Action Map

    private void SwitchToInputActionMap(InputActionMapType type)
    {
        _inputActions.Gameplay.Disable();
        _inputActions.Inventory.Disable();
        _inputActions.PausedGamplay.Disable();
        _inputActions.GameOverScreen.Disable();
        _inputActions.Menu.Disable();

        ActionMapTypeToActionMap(type).Enable();
        _currentActionMap = type;
    }

    private InputActionMap ActionMapTypeToActionMap(InputActionMapType type)
    {
        switch (type)
        {
            case InputActionMapType.Gameplay:
                return _inputActions.Gameplay;
            case InputActionMapType.Inventory:
                return _inputActions.Inventory;
            case InputActionMapType.PausedGameplay:
                return _inputActions.PausedGamplay;
            case InputActionMapType.GameOverScreen:
                return _inputActions.GameOverScreen;
            case InputActionMapType.Menu:
                return _inputActions.Menu;
            default:
                throw new Exception("Invalid input action type: " + type);
        }
    }

    #endregion
    
    public void OnGameEnded()
    {
        IsAimingEnabled = false;
        IsPausingGameplayEnabled = false;
        IsOpeningInventoryEnabled = false;
        ForceCloseInventory();
        SwitchToInputActionMap(InputActionMapType.GameOverScreen);
    }

    private void OnTogglePauseGameplay(InputAction.CallbackContext ctx)
    {
        if (_currentActionMap != InputActionMapType.Gameplay && _currentActionMap != InputActionMapType.PausedGameplay)
        {
            return;
        }
        var targetActionMapType = _currentActionMap == InputActionMapType.Gameplay ? InputActionMapType.PausedGameplay : InputActionMapType.Gameplay;
        if (!IsPausingGameplayEnabled && targetActionMapType == InputActionMapType.PausedGameplay)
        {
            return;
        }
        SwitchToInputActionMap(targetActionMapType);
        TogglePauseGameplayPerformed?.Invoke();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
        {
            _currentInputDevice = device;
        }
    }

}