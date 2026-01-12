using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public class LocalInputHandler : MonoBehaviour
{
    public PlayerInputActions InputActions { get; private set; }
    public InputActionMapType CurrentActionMap { get; private set; }
    public InputDevice CurrentInputDevice { get; private set; }

    private InputUser _inputUser;

    private IDisposable _onAnyButtonPressSubscription;

    private const int MaxUnpairedDeviceCount = 10;

    // aim
    [SerializeField] private AnimationCurve _gamepadStickResponseCurve = new AnimationCurve(
       new Keyframe(0, 0),
       new Keyframe(0.5f, 0.2f),
       new Keyframe(1, 1));

    private float _mouseAimRadius;
    private Vector2 _aimVector;
    private Vector2 _initialMouseAimPosition;
    private bool _isAiming;
    private bool _isInitialMouseAimPositionSet;

    public static readonly Vector2 DefaultAimStartPosition = new Vector2(-1, -1);
    public bool IsAimingEnabled { get; set; }

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;

    // inventory
    public bool IsOpeningInventoryEnabled { get; set; }

    public event Action ToggleInventoryPerformed;
    public event Action ToggleInventoryCreateDestroyPerformed;

    // pause
    public bool IsOpeningGameplayMenuEnabled { get; set; }

    public event Action ToggleGameplayMenuPerformed;

    // map
    public event Action<bool> ShowMapToggled;

    // skip
    public event Action ActionSkipped;

    private void Awake()
    {
        InputActions = new PlayerInputActions();
        InputActions.Enable();
        SubscribeToInputActions();

        if (Gamepad.current != null) // prioritize gamepad
        {
            CurrentInputDevice = Gamepad.current;
        }
        else if (InputSystem.devices.Count > 0)
        {
            CurrentInputDevice = InputSystem.devices[0];
        }
        if (CurrentInputDevice != null)
        {
            _inputUser = InputUser.PerformPairingWithDevice(CurrentInputDevice);
            _inputUser.AssociateActionsWithUser(InputActions);
        }

        InputUser.listenForUnpairedDeviceActivity = MaxUnpairedDeviceCount;

        InputSystem.onDeviceChange += OnDeviceChange;
        _onAnyButtonPressSubscription = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        _onAnyButtonPressSubscription?.Dispose();
    }

    #region Input Action Map Management

    public void SwitchToInputActionMap(InputActionMapType type)
    {
        InputActions.Gameplay.Disable();
        InputActions.Inventory.Disable();
        InputActions.GameplayMenu.Disable();
        InputActions.Menu.Disable();

        var targetMap = ActionMapTypeToActionMap(type);
        targetMap.Enable();

        InputActions.UI.Enable();

        CurrentActionMap = type;
        RebindToCurrentDevice();
    }

    private InputActionMap ActionMapTypeToActionMap(InputActionMapType type)
    {
        return type switch
        {
            InputActionMapType.Gameplay => InputActions.Gameplay,
            InputActionMapType.Inventory => InputActions.Inventory,
            InputActionMapType.GameplayMenu => InputActions.GameplayMenu,
            InputActionMapType.GameOverScreen => InputActions.GameOver,
            InputActionMapType.Menu => InputActions.Menu,
            _ => throw new Exception("Invalid input action type: " + type),
        };
    }

    private void SubscribeToInputActions()
    {
        InputActions.Gameplay.Aim.performed += OnAimPerformed;
        InputActions.Gameplay.Aim.canceled += OnCancelAimingPerformed;
        InputActions.Gameplay.ReleaseImpulse.started += OnImpulseReleaseStarted;
        InputActions.Gameplay.ReleaseImpulse.canceled += OnImpulseReleaseEnded;
        InputActions.Gameplay.Cancel.started += OnCancelAimingPerformed;

        InputActions.Gameplay.ShowGameplayMenu.started += OnToggleGameplayMenu;
        InputActions.GameplayMenu.ResumeGameplay.started += OnToggleGameplayMenu;

        InputActions.Gameplay.ToggleInventory.started += OnToggleInventory;
        InputActions.Inventory.ToggleInventory.started += OnToggleInventory;
        InputActions.Inventory.ToggleCreateDestroy.started += OnToggleCreateDestroy;

        InputActions.Gameplay.ShowMap.started += OnShowMapPerformed;
        InputActions.Gameplay.ShowMap.canceled += OnShowMapCancelled;

        InputActions.Gameplay.SkipAction.started += OnSkipActionPerformed;
    }


    #endregion

    #region Device Change

    private void OnAnyButtonPress(InputControl control)
    {
        var device = control.device;

        if (device == CurrentInputDevice)
            return;

        CurrentInputDevice = device;
        RebindToCurrentDevice();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
        {
            CurrentInputDevice = device;
            RebindToCurrentDevice();
        }
    }

    private void RebindToCurrentDevice()
    {
        if (!CurrentInputDevice.added)
            return;

        if (CurrentInputDevice is Keyboard || CurrentInputDevice is Mouse)
        {
            Cursor.visible = true;
        }

        if (_inputUser.valid)
        {
            _inputUser.UnpairDevicesAndRemoveUser();
        }

        _inputUser = InputUser.PerformPairingWithDevice(CurrentInputDevice);
        _inputUser.AssociateActionsWithUser(InputActions);

        Debug.Log($"Input paired with {CurrentInputDevice.displayName}");
    } 

    #endregion

    #region Aim

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
        }
    }

    private void HandleGamepadAiming(InputAction.CallbackContext ctx)
    {
        Cursor.visible = false;
        _aimVector = -GetGamepadStickValue(ctx);
        if (!_isAiming)
        {
            AimStarted?.Invoke(DefaultAimStartPosition);
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

    private void OnImpulseReleaseStarted(InputAction.CallbackContext ctx)
    {
        if (!IsAimingEnabled || (ctx.control.device is not Mouse && ctx.control.device is not Keyboard))
            return;

        _isAiming = true;
        _isInitialMouseAimPositionSet = false;
        _aimVector = Vector2.zero;
        var initialPos = DefaultAimStartPosition;
        if (ctx.control.device is Mouse)
        {
            initialPos = Mouse.current.position.ReadValue();
            _mouseAimRadius = (AimCircleUI.OuterRadiusPercent - AimCircleUI.InnerRadiusPercent) * Screen.width;
        }
        AimStarted?.Invoke(initialPos);
        Cursor.visible = false;
    }

    private void OnImpulseReleaseEnded(InputAction.CallbackContext ctx)
    {
        if (!IsAimingEnabled || !_isAiming)
            return;

        if (ctx.control.device is Mouse || ctx.control.device is Keyboard)
        {
            Cursor.visible = true;
        }
        if (_aimVector.Approximately(Vector2.zero))
        {
            CancelAiming();
            return;
        }

        _isAiming = false;
        ImpulseReleased?.Invoke(_aimVector);
    }

    private Vector2 GetGamepadStickValue(InputAction.CallbackContext ctx)
    {
        Vector2 raw = ctx.ReadValue<Vector2>();
        float mag = raw.magnitude;
        Vector2 normalized = raw.normalized;
        float adjustedMag = _gamepadStickResponseCurve.Evaluate(mag);
        return normalized * adjustedMag;
    }

    private void OnCancelAimingPerformed(InputAction.CallbackContext ctx)
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

    public void ForceCancelAiming()
    {
        CancelAiming();
    }

    #endregion

    #region Inventory

    private void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        ToggleInventoryAndGameplayActionMap();
    }

    private void ToggleInventoryAndGameplayActionMap()
    {
        var targetActionMapType = CurrentActionMap == InputActionMapType.Gameplay ? InputActionMapType.Inventory : InputActionMapType.Gameplay;
        if (!IsOpeningInventoryEnabled && targetActionMapType == InputActionMapType.Inventory)
        {
            return;
        }
        CancelAiming();
        SwitchToInputActionMap(targetActionMapType);
        ToggleInventoryPerformed?.Invoke();
    }

    public void ForceCloseInventory()
    {
        if (CurrentActionMap == InputActionMapType.Inventory)
        {
            ToggleInventoryAndGameplayActionMap();
        }
    }

    private void OnToggleCreateDestroy(InputAction.CallbackContext ctx)
    {
        ToggleInventoryCreateDestroyPerformed?.Invoke();
    }

    #endregion

    #region Pause / Resume

    public void ToggleGameplayMenu()
    {
        if (CurrentActionMap != InputActionMapType.Gameplay && CurrentActionMap != InputActionMapType.GameplayMenu)
        {
            return;
        }
        var targetActionMapType = CurrentActionMap == InputActionMapType.Gameplay ? InputActionMapType.GameplayMenu : InputActionMapType.Gameplay;
        if (!IsOpeningGameplayMenuEnabled && targetActionMapType == InputActionMapType.GameplayMenu)
        {
            return;
        }
        SwitchToInputActionMap(targetActionMapType);
        GameServices.GameStateManager.TogglePauseResumeGameplay(); //TODO: check ui if pause not possible
    }

    private void OnToggleGameplayMenu(InputAction.CallbackContext ctx)
    {
        ToggleGameplayMenu();
    }

    #endregion

    #region Show Map

    private void OnShowMapPerformed(InputAction.CallbackContext ctx)
    {
        ShowMapToggled?.Invoke(true);
    }

    private void OnShowMapCancelled(InputAction.CallbackContext ctx)
    {
        ShowMapToggled?.Invoke(false);
    }

    #endregion

    #region Skip

    private void OnSkipActionPerformed(InputAction.CallbackContext ctx)
    {
        ActionSkipped?.Invoke();
    }

    #endregion

}
