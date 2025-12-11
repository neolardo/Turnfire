using System;
using UnityEngine.InputSystem;

public class LocalMenuInput : LocalInputBase
{
    public event Action MenuConfirmPerformed;
    public event Action MenuBackPerformed;
    public event Action MenuIncrementValuePerformed;
    public event Action MenuDecrementValuePerformed;

    protected override void Awake()
    {
        base.Awake();
        SwitchToInputActionMap(InputActionMapType.Menu);
    }

    protected override void SubscribeToInputEvents()
    {
        base.SubscribeToInputEvents();
        _inputActions.Menu.Back.performed += OnMenuBackPerformed;
        _inputActions.Menu.Confirm.performed += OnMenuConfirmPerformed;
        _inputActions.Menu.IncrementValue.performed += OnMenuIncrementValuePerformed;
        _inputActions.Menu.DecrementValue.performed += OnMenuDecrementValuePerformed;
    }


    private void OnMenuBackPerformed(InputAction.CallbackContext ctx)
    {
        MenuBackPerformed?.Invoke();
    }
    private void OnMenuConfirmPerformed(InputAction.CallbackContext ctx)
    {
        MenuConfirmPerformed?.Invoke();
    }
    private void OnMenuIncrementValuePerformed(InputAction.CallbackContext ctx)
    {
        MenuIncrementValuePerformed?.Invoke();
    }
    private void OnMenuDecrementValuePerformed(InputAction.CallbackContext ctx)
    {
        MenuDecrementValuePerformed?.Invoke();
    }

}
