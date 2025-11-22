using System;
using UnityEngine.InputSystem;

public class LocalMenuInput : LocalInputBase
{
    public event Action MenuConfirmPerformed;
    public event Action MenuBackPerformed;
    public event Action MenuNavigateUpPerformed;
    public event Action MenuNavigateDownPerformed;
    public event Action MenuNavigateLeftPerformed;
    public event Action MenuNavigateRightPerformed;
    public event Action MenuToggleCheckboxPerformed;

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
        _inputActions.Menu.NavigateUp.performed += OnMenuNavigateUpPerformed;
        _inputActions.Menu.NavigateDown.performed += OnMenuNavigateDownPerformed;
        _inputActions.Menu.NavigateRight.performed += OnMenuNavigateRightPerformed;
        _inputActions.Menu.NavigateLeft.performed += OnMenuNavigateLeftPerformed;
        _inputActions.Menu.ToggleCheckbox.performed += OnMenuToggleCheckboxPerformed;
    }


    private void OnMenuBackPerformed(InputAction.CallbackContext ctx)
    {
        MenuBackPerformed?.Invoke();
    }
    private void OnMenuConfirmPerformed(InputAction.CallbackContext ctx)
    {
        MenuConfirmPerformed?.Invoke();
    }
    private void OnMenuNavigateUpPerformed(InputAction.CallbackContext ctx)
    {
        MenuNavigateUpPerformed?.Invoke();
    }
    private void OnMenuNavigateDownPerformed(InputAction.CallbackContext ctx)
    {
        MenuNavigateDownPerformed?.Invoke();
    }
    private void OnMenuNavigateRightPerformed(InputAction.CallbackContext ctx)
    {
        MenuNavigateRightPerformed?.Invoke();
    }
    private void OnMenuNavigateLeftPerformed(InputAction.CallbackContext ctx)
    {
        MenuNavigateLeftPerformed?.Invoke();
    }
    private void OnMenuToggleCheckboxPerformed(InputAction.CallbackContext ctx)
    {
        MenuToggleCheckboxPerformed?.Invoke();
    }

}
