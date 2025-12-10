using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalGameplayInput : LocalInputBase, IGameplayInputSource
{
    [SerializeField] private AnimationCurve _gamepadStickResponseCurve =
    new AnimationCurve(
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
    public bool IsOpeningInventoryEnabled { get; set; }
    public bool IsPausingGameplayEnabled { get; set; }

    //gameplay
    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;
    public event Action ActionSkipped;
    public event Action ItemUsed;
    public event Action SelectedItemUsed; //TODO
    public event Action<Item> ItemSwitched;  //TODO! implement
    //inventory
    public event Action ToggleInventoryPerformed;
    public event Action ToggleInventoryCreateDestroyPerformed;
    public event Action TogglePauseGameplayPerformed;
    public event Action SelectInventorySlotPerformed;
    // map
    public event Action<bool> ShowMapToggled;
    //pause
    public event Action PausedScreenConfirmPerformed;
    //game over
    public event Action GameOverScreenConfirmPerformed;

    protected override void Awake()
    {
        base.Awake();
        SwitchToInputActionMap(InputActionMapType.Gameplay);
    }

    protected override void SubscribeToInputEvents()
    {
        base.SubscribeToInputEvents();
        _inputActions.Gameplay.Aim.performed += OnAimPerformed;
        _inputActions.Gameplay.Aim.canceled += OnCancelAimingPerformed;
        _inputActions.Gameplay.ReleaseImpulse.started += OnImpulseReleaseStarted;
        _inputActions.Gameplay.ReleaseImpulse.canceled += OnImpulseReleaseEnded;
        _inputActions.Gameplay.Cancel.started += OnCancelAimingPerformed;
        _inputActions.Gameplay.SkipAction.started += OnSkipActionPerformed;
        _inputActions.Gameplay.ToggleInventory.started += OnToggleInventory;
        _inputActions.Gameplay.PauseGameplay.started += OnTogglePauseGameplay;
        _inputActions.Gameplay.ShowMap.started += OnShowMapPerformed;
        _inputActions.Gameplay.ShowMap.canceled += OnShowMapCancelled;
        _inputActions.PausedGamplay.ResumeGameplay.started += OnTogglePauseGameplay;
        _inputActions.PausedGamplay.Confirm.performed += OnPausedGameplayConfirmPressed;
        _inputActions.Inventory.ToggleInventory.started += OnToggleInventory;
        _inputActions.Inventory.ToggleCreateDestroy.started += OnToggleCreateDestroy;
        _inputActions.Inventory.SelectInventorySlot.started += OnSelectInventorySlot;
        _inputActions.GameOverScreen.Confirm.performed += OnGameOverScreenConfirmPerformed;

    }

    #region Game States

    public void PrepareForGameStart()
    {
        DisableInputBeforeGameStart();
    }

    private void DisableInputBeforeGameStart()
    {
        IsAimingEnabled = false;
        IsPausingGameplayEnabled = false;
        IsOpeningInventoryEnabled = false;
    }

    public void OnGameStarted()
    {
        IsPausingGameplayEnabled = true;
    }

    public void OnGameEnded()
    {
        IsAimingEnabled = false;
        IsPausingGameplayEnabled = false;
        IsOpeningInventoryEnabled = false;
        ForceCloseInventory();
        SwitchToInputActionMap(InputActionMapType.GameOverScreen);
    }

    #endregion

    #region Gameplay

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
        _aimVector = GetGamepadStickValue(ctx);
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

    private void OnSkipActionPerformed(InputAction.CallbackContext ctx)
    {
        ActionSkipped?.Invoke();
    }

    public void RequestInputForAction(CharacterActionStateType action) { } // local input is provided automatically


    public void Initialize(Team team) { } // no need to initialize


    #endregion

    #region Inventory

    private void OnSelectInventorySlot(InputAction.CallbackContext ctx)
    {
        SelectInventorySlotPerformed?.Invoke();
    }

    private void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        ToggleInventoryAndGameplayActionMap();
    }

    private void ToggleInventoryAndGameplayActionMap()
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

    public void ForceCloseInventory()
    {
        if (_currentActionMap == InputActionMapType.Inventory)
        {
            ToggleInventoryAndGameplayActionMap();
        }
    }

    private void OnToggleCreateDestroy(InputAction.CallbackContext ctx)
    {
        ToggleInventoryCreateDestroyPerformed?.Invoke();
    }

    #endregion

    #region Game Over

    private void OnGameOverScreenConfirmPerformed(InputAction.CallbackContext ctx)
    {
        GameOverScreenConfirmPerformed?.Invoke();
    }

    #endregion

    #region Pause / Resume

    public void TogglePauseResumeGameplay()
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

    private void OnPausedGameplayConfirmPressed(InputAction.CallbackContext ctx)
    {
        PausedScreenConfirmPerformed?.Invoke();
    }

    private void OnTogglePauseGameplay(InputAction.CallbackContext ctx)
    {
        TogglePauseResumeGameplay();
    }


    #endregion

}