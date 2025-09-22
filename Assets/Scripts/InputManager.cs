using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    [SerializeField] private AnimationCurve gamepadStickResponseCurve =
    new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 0.2f),
        new Keyframe(1, 1));

    private float _mouseAimRadius;
    private Vector2 _aimVector;
    private Vector2 _initialAimPosition;
    private bool _isAiming;
    private bool _initialAimPositionSet;
    private PlayerInputActions _inputActions;

    public bool IsAimingEnabled { get; set; }

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        SubscribeToInputEvents();
    }

    private void SubscribeToInputEvents()
    {
        _inputActions.Player.Aim.performed += OnAimPerformed;
        _inputActions.Player.Aim.canceled += OnAimCancelled;
        _inputActions.Player.ReleaseImpulse.started += OnImpulseReleaseStarted;
        _inputActions.Player.ReleaseImpulse.canceled += OnImpulseReleaseEnded;
        _inputActions.Player.Cancel.started += OnCancelPerformed;
        //_inputActions.Player.Shoot.started += OnJumpStarted; //TODO
        //_inputActions.Player.Shoot.canceled += OnJumpStarted;
    }

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
        AimChanged?.Invoke(_aimVector);
    }


    private void HandleMouseAiming(InputAction.CallbackContext ctx)
    {
        if (_isAiming)
        {
            var pos = ctx.ReadValue<Vector2>();
            if (!_initialAimPositionSet)
            {
                _initialAimPositionSet = true;
                _initialAimPosition = pos;
            }
            _aimVector = _initialAimPosition - pos;
            if (_aimVector.magnitude > _mouseAimRadius)
            {
                _aimVector = _aimVector.normalized * _mouseAimRadius;
                Mouse.current.WarpCursorPosition(_initialAimPosition - _aimVector);
            }
            _aimVector = _aimVector / _mouseAimRadius;
            AimChanged?.Invoke(_aimVector);
        }
    }


    private void OnAimCancelled(InputAction.CallbackContext ctx)
    {
        if (!IsAimingEnabled)
            return;

        if (ctx.control.device is Mouse)
        {
            _aimVector = Vector2.zero;
            if (_isAiming)
            {
                AimChanged?.Invoke(_aimVector);
                _initialAimPositionSet = false;
            }
        }
    }

    private void OnImpulseReleaseStarted(InputAction.CallbackContext ctx)
    {
        if (!IsAimingEnabled)
            return;

        _isAiming = true;
        _initialAimPositionSet = false;
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
        if (_isAiming)
        {
            _isAiming = false;
            Cursor.visible = true;
            AimCancelled?.Invoke();
        }
    }

    private void OnEnable() => _inputActions.Player.Enable();
    private void OnDisable() => _inputActions.Player.Disable();
}