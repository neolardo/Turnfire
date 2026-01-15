using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;

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

    public bool IsLocal { get; private set; }

    public event Action<Vector2> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action<Vector2> ImpulseReleased;
    public event Action AimCancelled;
    public event Action ActionSkipped;
    public event Action<ItemUsageContext> SelectedItemUsed;
    public event Action<int> ItemSelected;

    private void OnEnable()
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

    public override void OnNetworkDespawn()
    {
        GameServices.TurnStateManager.GameStarted -= OnGameStarted;
        GameServices.TurnStateManager.GameEnded -= OnGameEnded;
        base.OnNetworkDespawn();
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
    public void InitializeIsLocal(bool isLocal)
    {
        IsLocal = isLocal;
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

    public void OnGameEnded(Team winner)
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
        InvokeImpulseReleasedServerRpc(impulse);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeImpulseReleasedServerRpc(Vector2 impulse)
    {
        ImpulseReleased?.Invoke(impulse);
    }

    private void InvokeAimStarted(Vector2 initialPosition)
    {
        if (!IsOwner)
        {
            return;
        }
        InvokeAimStartedServerRpc(initialPosition);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeAimStartedServerRpc(Vector2 initialPosition)
    {
        AimStarted?.Invoke(initialPosition);
    }

    private void InvokeAimChanged(Vector2 aimVector)
    {
        if (!IsOwner)
        {
            return;
        }
        InvokeAimChangedServerRpc(aimVector);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeAimChangedServerRpc(Vector2 aimVector)
    {
        AimChanged?.Invoke(aimVector);
    }

    private void InvokeAimCancelled()
    {
        if (!IsOwner)
        {
            return;
        }
        InvokeAimCancelledServerRpc();
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeAimCancelledServerRpc()
    {
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
        InvokeActionSkippedServerRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeActionSkippedServerRpc()
    {
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

    private void InvokeItemSelected(ItemInstance item)
    {
        if (!IsOwner)
        {
            return;
        }
        InvokeItemSelectedServerRpc(item.InstanceId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void InvokeItemSelectedServerRpc(int itemInstanceId)
    {
        ItemSelected?.Invoke(itemInstanceId);
    }

    #endregion

    #region Gameplay Menu

    private void OnIsOpeningGameplayMenuEnabledChanged(bool oldValue, bool newValue)
    {
        _inputHandler.IsOpeningGameplayMenuEnabled = newValue;
    }

    #endregion
}
