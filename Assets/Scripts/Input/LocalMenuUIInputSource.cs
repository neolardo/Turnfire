using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMenuUIInputSource : MonoBehaviour
{
    private LocalInputHandler _inputHandler;

    public event Action MenuConfirmPerformed;
    public event Action MenuBackPerformed;
    public event Action MenuIncrementValuePerformed;
    public event Action MenuDecrementValuePerformed;

    private void Start()
    {
        _inputHandler = FindFirstObjectByType<LocalInputHandler>();
        _inputHandler.SwitchToInputActionMap(InputActionMapType.Menu);
        _inputHandler.IsAimingEnabled = false;
        _inputHandler.IsActionSkippingEnabled = false;
        _inputHandler.IsOpeningGameplayMenuEnabled = false;
        _inputHandler.IsOpeningInventoryEnabled = false;
        SubscribeToInputEvents();
    }
    private void SubscribeToInputEvents()
    {
        var inputActions = _inputHandler.InputActions;
        inputActions.Menu.Back.performed += OnMenuBackPerformed;
        inputActions.Menu.Confirm.performed += OnMenuConfirmPerformed;
        inputActions.Menu.IncrementValue.performed += OnMenuIncrementValuePerformed;
        inputActions.Menu.DecrementValue.performed += OnMenuDecrementValuePerformed;
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
