using System;
using UnityEngine;

public class OfflineHumanTeamInputSource : MonoBehaviour, ITeamInputSource
{
    private LocalInputHandler _inputHandler;
    public bool IsAimingEnabled
    {
        get 
        {
            return _inputHandler.IsAimingEnabled;
        }
        set 
        { 
            _inputHandler.IsAimingEnabled = value;
        }
    }

    public bool IsOpeningInventoryEnabled
    {
        get
        {
            return _inputHandler.IsOpeningInventoryEnabled;
        }
        set
        {
            _inputHandler.IsOpeningInventoryEnabled = value;
        }
    }

    public bool IsOpeningGameplayMenuEnabled
    {
        get
        {
            return _inputHandler.IsOpeningGameplayMenuEnabled;
        }
        set
        {
            _inputHandler.IsOpeningGameplayMenuEnabled = value;
        }
    }
    public bool IsActionSkippingEnabled
    {
        get
        {
            return _inputHandler.IsActionSkippingEnabled;
        }
        set
        {
            _inputHandler.IsActionSkippingEnabled = value;
        }
    }

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;
    public event Action ActionSkipped;
    public event Action<ItemUsageContext> SelectedItemUsed;
    public event Action<int> ItemSelected;

    private void Start()
    {
        _inputHandler = FindFirstObjectByType<LocalInputHandler>();
        _inputHandler.SwitchToInputActionMap(InputActionMapType.Gameplay);
        SubscribeToInputEvents();
        if (GameServices.IsInitialized)
        {
            OnGameServicesInitialized();
        }
        else
        {
            GameServices.Initialized += OnGameServicesInitialized;
        }
        DisableInputBeforeGameStart();
    }

    private void OnGameServicesInitialized()
    {
        GameServices.TurnStateManager.GameStarted += OnGameStarted;
        GameServices.TurnStateManager.GameEnded += OnGameEnded;
    }

    private void OnDestroy()
    {
        GameServices.TurnStateManager.GameStarted -= OnGameStarted;
        GameServices.TurnStateManager.GameEnded -= OnGameEnded;
    }

    private void SubscribeToInputEvents()
    {
        var inputActions = _inputHandler.InputActions;
        _inputHandler.ImpulseReleased += InvokeImpulseReleased;
        _inputHandler.AimStarted += InvokeAimStarted;
        _inputHandler.AimChanged += InvokeAimChanged;
        _inputHandler.AimCancelled += InvokeAimCancelled;
        _inputHandler.ActionSkipped += InvokeActionSkipped;
        _inputHandler.InventoryItemSelected += InvokeItemSelected;
    }

    public void RequestAction(CharacterActionStateType action)
    {
        // local input is provided automatically
    }

    #region Game States

    private void DisableInputBeforeGameStart()
    {
        IsAimingEnabled = false;
        IsOpeningGameplayMenuEnabled = false;
        IsOpeningInventoryEnabled = false;
    }

    public void OnGameStarted()
    {
        IsOpeningGameplayMenuEnabled = true;
    }

    public void OnGameEnded(Team winner)
    {
        IsAimingEnabled = false;
        IsOpeningGameplayMenuEnabled = false;
        IsOpeningInventoryEnabled = false;
        ForceCloseInventory();
        _inputHandler.SwitchToInputActionMap(InputActionMapType.GameOverScreen);
    }

    #endregion

    #region Aiming

    public void ForceCancelAiming()
    {
        _inputHandler.ForceCancelAiming();
    }
    private void InvokeImpulseReleased(Vector2 impulse)
    {
        Debug.Log("impulse released");
        ImpulseReleased?.Invoke(impulse);
    }
    private void InvokeAimStarted(Vector2 initialPosition)
    {
        AimStarted?.Invoke(initialPosition);
    }
    private void InvokeAimChanged(Vector2 aimVector)
    {
        AimChanged?.Invoke(aimVector);
    }
    private void InvokeAimCancelled()
    {
        AimCancelled?.Invoke();
    }
    private void InvokeItemSelected(ItemInstance item)
    {
        ItemSelected?.Invoke(item.InstanceId);
    }

    #endregion

    #region Skip

    private void InvokeActionSkipped()
    {
        ActionSkipped?.Invoke();
    }

    #endregion

    #region Inventory

    public void ForceCloseInventory()
    {
        _inputHandler.ForceCloseInventory();
    }

    #endregion

}