using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private float _mouseAimRadius;
    private Vector2 _aimVector;
    private Vector2 _initialAimPosition;
    private bool _isAiming;
    private bool _initialAimPositionSet;
    private PlayerInputActions _inputActions;

    public bool IsCharging => _isAiming;

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        SubscribeToInputEvents();
    }

    private void SubscribeToInputEvents()
    {
        _inputActions.Player.Aim.performed += OnAimPerformed;
        _inputActions.Player.Aim.canceled += OnAimCancelled;
        _inputActions.Player.Jump.started += OnJumpStarted;
        _inputActions.Player.Jump.canceled += OnJumpCancelled;
    }

    private void OnAimPerformed(InputAction.CallbackContext ctx)
    {
        var pos = ctx.ReadValue<Vector2>();
        if (!_initialAimPositionSet)
        {
            _initialAimPositionSet = true;
            _initialAimPosition = pos;
        }
        if (_isAiming)
        {
            _aimVector = _initialAimPosition - pos;
            if (ctx.control.device is Mouse)
            {
                if (_aimVector.magnitude > _mouseAimRadius)
                {
                    _aimVector = _aimVector.normalized * _mouseAimRadius;
                    Mouse.current.WarpCursorPosition(_initialAimPosition - _aimVector);
                }
                _aimVector = _aimVector / _mouseAimRadius;
            }
            AimChanged?.Invoke(_aimVector);
        }
    }


    private void OnAimCancelled(InputAction.CallbackContext ctx)
    {
        _aimVector = Vector2.zero;
        if (_isAiming)
        {
            AimChanged?.Invoke(_aimVector);
            _initialAimPositionSet = false;
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        _isAiming = true;
        _initialAimPositionSet = false;
        var initialPos = new Vector2(-1, -1);
        if(ctx.control.device is Mouse)
        {
            var mouse = (ctx.control.device as Mouse);
            initialPos = new Vector2( mouse.position.x.magnitude, mouse.position.y.magnitude);
            _mouseAimRadius = (Constants.AimCircleOuterRadiusPercent - Constants.AimCircleInnerRadiusPercent) * Screen.width;
        }
        AimStarted?.Invoke(initialPos);
        Cursor.visible = false;
    }

    private void OnJumpCancelled(InputAction.CallbackContext ctx)
    {
        _isAiming = false;
        ImpulseReleased?.Invoke(_aimVector);
        Cursor.visible = true;
    }

    private void OnEnable() => _inputActions.Player.Enable();
    private void OnDisable() => _inputActions.Player.Disable();
}