using System;
using Unity.Netcode;
using UnityEngine;

public class OnlineHumanTeamInputSource : NetworkBehaviour, ITeamInputSource
{
    private LocalInputHandler _inputHandler;
    private NetworkVariable<bool> _isAimingEnabled = new();
    public bool IsAimingEnabled
    {
        get
        {
            return _isAimingEnabled.Value;
        }
        set
        {
            if(!IsServer)
            {
                return;
            }
            _isAimingEnabled.Value = value;
        }
    }

    private NetworkVariable<bool> _isOpeningInventoryEnabled = new();
    public bool IsOpeningInventoryEnabled
    {
        get
        {
            return _isOpeningInventoryEnabled.Value;
        }
        set
        {
            if (!IsServer)
            {
                return;
            }
            _isOpeningInventoryEnabled.Value = value;
        }
    }

    private NetworkVariable<bool> _isOpeningGameplayMenuEnabled = new();
    public bool IsOpeningGameplayMenuEnabled
    {
        get
        {
            return _isOpeningGameplayMenuEnabled.Value;
        }
        set
        {
            if (!IsServer)
            {
                return;
            }
            _isOpeningGameplayMenuEnabled.Value = value;
        }
    }

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;
    public event Action ActionSkipped;
    public event Action<ItemUsageContext> SelectedItemUsed;
    public event Action<ItemInstance> ItemSelected;

    private void Start()
    {
        _inputHandler = FindFirstObjectByType<LocalInputHandler>();
        if (!IsOwner)
        {
            return;
        }
        _inputHandler.SwitchToInputActionMap(InputActionMapType.Gameplay);
        SubscribeToInputEvents();

        _isAimingEnabled.OnValueChanged += OnIsAimingEnabledChanged;
        _isOpeningInventoryEnabled.OnValueChanged += OnIsOpeningInventoryEnabledChanged;
        _isOpeningGameplayMenuEnabled.OnValueChanged += OnIsOpeningGameplayMenuEnabledChanged;
        GameServices.TurnStateManager.GameStarted += OnGameStarted;
        GameServices.TurnStateManager.GameEnded += (_) => OnGameEnded();
        DisableInputBeforeGameStart();
    }
    private void SubscribeToInputEvents()
    {
        var inputActions = _inputHandler.InputActions;
        _inputHandler.ImpulseReleased += InvokeImpulseReleased;
        _inputHandler.AimStarted += InvokeAimStarted;
        _inputHandler.AimChanged += InvokeAimChanged;
        _inputHandler.AimCancelled += InvokeAimCancelled;
        _inputHandler.ActionSkipped += InvokeActionSkipped;
    }

    public void RequestAction(CharacterActionStateType action)
    {
        // local input is provided automatically
    }

    #region Game States

    private void DisableInputBeforeGameStart()
    {
        if(!IsServer)
        {
            return;
        }
        IsAimingEnabled = false;
        IsOpeningGameplayMenuEnabled = false;
        IsOpeningInventoryEnabled = false;
    }

    public void OnGameStarted()
    {
        if (!IsServer)
        {
            return;
        }
        IsOpeningGameplayMenuEnabled = true;
    }

    public void OnGameEnded()
    {
        ForceCloseInventory();
        _inputHandler.SwitchToInputActionMap(InputActionMapType.GameOverScreen);
        if (!IsServer)
        {
            return;
        }
        IsAimingEnabled = false;
        IsOpeningGameplayMenuEnabled = false;
        IsOpeningInventoryEnabled = false;
    }

    #endregion

    #region Aiming

    public void ForceCancelAiming()
    {
        if (!IsServer)
        {
            return;
        }
        ForceCancelAimingOwnerRpc();
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Server)]
    private void ForceCancelAimingOwnerRpc()
    {
        _inputHandler.ForceCancelAiming();
    }

    private void InvokeImpulseReleased(Vector2 impulse)
    {
        if (!IsOwner)
        {
            return;
        }
        ImpulseReleased?.Invoke(impulse);
        InvokeImpulseReleasedServerRpc(impulse);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeImpulseReleasedServerRpc(Vector2 impulse)
    {
        if (IsOwner)
        {
            return;
        }
        ImpulseReleased?.Invoke(impulse);
    }

    private void InvokeAimStarted(Vector2 initialPosition)
    {
        if (!IsOwner)
        {
            return;
        }
        AimStarted?.Invoke(initialPosition);
        InvokeAimStartedServerRpc(initialPosition);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeAimStartedServerRpc(Vector2 initialPosition)
    {
        if (IsOwner)
        {
            return;
        }
        AimStarted?.Invoke(initialPosition);
    }

    private void InvokeAimChanged(Vector2 aimVector)
    {
        if (!IsOwner)
        {
            return;
        }
        AimChanged?.Invoke(aimVector);
        InvokeAimChangedServerRpc(aimVector);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeAimChangedServerRpc(Vector2 aimVector)
    {
        if(IsOwner)
        {
            return;
        }
        AimChanged?.Invoke(aimVector);
    }

    private void InvokeAimCancelled()
    {
        if (!IsOwner)
        {
            return;
        }
        AimCancelled?.Invoke();
        InvokeAimCancelledServerRpc();
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeAimCancelledServerRpc()
    {
        if (IsOwner)
        {
            return;
        }
        AimCancelled?.Invoke();
    }

    private void OnIsAimingEnabledChanged(bool oldValue, bool newValue)
    {
        _inputHandler.IsAimingEnabled = newValue;
    }

    #endregion

    #region Skip

    private void InvokeActionSkipped()
    {
        if (!IsOwner)
        {
            return;
        }
        ActionSkipped?.Invoke();
        InvokeActionSkippedServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeActionSkippedServerRpc()
    {
        if (IsOwner)
        {
            return;
        }
        ActionSkipped?.Invoke();
    }

    #endregion

    #region Inventory

    public void ForceCloseInventory()
    {
        if (!IsServer)
        {
            return;
        }
        ForceCloseInventoryOwnerRpc();
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Server)]
    private void ForceCloseInventoryOwnerRpc()
    {
        _inputHandler.ForceCloseInventory();
    }

    private void OnIsOpeningInventoryEnabledChanged(bool oldValue, bool newValue)
    {
        _inputHandler.IsOpeningInventoryEnabled = newValue;
    }

    #endregion

    #region Gameplay Menu

    private void OnIsOpeningGameplayMenuEnabledChanged(bool oldValue, bool newValue)
    {
        _inputHandler.IsOpeningGameplayMenuEnabled = newValue;
    }

    #endregion
}
