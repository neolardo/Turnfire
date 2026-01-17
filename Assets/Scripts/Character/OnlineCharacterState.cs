using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineCharacterState : NetworkBehaviour, ICharacterState
{
    private Character _character;
    private CharacterDefinition _definition;
    private CharacterItemInventory _inventory;
    private CharacterArmorManager _armorManager;

    private NetworkVariable<int> _health = new NetworkVariable<int>();
    public int Health => _health.Value;
    public float NormalizedHealth => Health / (float)_definition.MaxHealth;

    public bool IsAlive => Health > 0;

    private NetworkVariable<NetworkItemUsageState> _itemUsageState = new NetworkVariable<NetworkItemUsageState>();
    public bool IsUsingSelectedItem => _itemUsageState.Value.IsInUse;
    public ItemInstance SelectedItem => _inventory.SelectedItem;

    private NetworkVariable<float> _jumpBoost = new NetworkVariable<float>();
    public float JumpBoost => _jumpBoost.Value;

    private NetworkVariable<bool> _isGrounded = new NetworkVariable<bool>(true);
    public bool IsGrounded => _isGrounded.Value;
    public float JumpStrength => CharacterDefinition.JumpStrength + JumpBoost;
    public Team Team { get; private set; }

    public event Action<float, int> HealthChanged;
    public event Action Healed;
    public event Action Died;
    public event Action<IDamageSourceDefinition> Hurt;
    public event Action<ArmorDefinition> Blocked;
    public event Action<Vector2> Jumped;
    public event Action<Vector2> Pushed;
    public event Action<bool> IsGroundedChanged;
    public event Action PreparedToJump;
    public event Action<Vector2> JumpAimChanged;
    public event Action JumpCancelled;

    public event Action<ItemInstance> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action AimCancelled;

    public event Action<ItemInstance, ItemUsageContext> ItemUsed;
    public event Action<ItemInstance> ItemSelected;

    public event Action<ArmorDefinition> ArmorEquipped;
    public event Action<ArmorDefinition> ArmorUnequipped;


    public void Initialize(Character character, CharacterDefinition characterDefinition, Team team)
    {
        _character = character;
        _definition = characterDefinition;
        Team = team;
        _armorManager = new CharacterArmorManager();
        _inventory = new CharacterItemInventory();
        _health.OnValueChanged += OnNetworkHealthValueChanged;
        _isGrounded.OnValueChanged += OnNetworkIsGroundedValueChanged;
        _itemUsageState.OnValueChanged += OnItemUsageStateChanged;
        if (!IsServer)
        {
            return; 
        }
        GetComponent<GroundChecker>().IsGroundedChanged += OnGroundCheckerIsGroundedChanged;
        _health.Value = _definition.MaxHealth;
        _armorManager.ArmorUnequipped += InvokeArmorUnequipped;
    }


    #region Health

    public void RequestTakeDamage(IDamageSourceDefinition damageSource, int damageValue)
    {
        if(!IsServer)
        {
            return;
        }    

        if (_armorManager.IsProtected)
        {
            var armor = _armorManager.BlockAttack();
            InvokeBlockedClientRpc(armor.Id);
        }
        else
        {
            InvokeHurtClientRpc(new NetworkDamageSourceDefinitionData(damageSource));
            _health.Value = Mathf.Max(0, Health - damageValue);
            if (!IsAlive)
            {
                InvokeDiedClientRpc();
            }
        }
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeHurtClientRpc(NetworkDamageSourceDefinitionData networkDamageSource)
    {
        Hurt?.Invoke(networkDamageSource.ToDamageSource());
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeBlockedClientRpc(int armorDefinitionId) 
    {
        Blocked?.Invoke(GameServices.ItemDatabase.GetById(armorDefinitionId) as ArmorDefinition); 
    }

    public void RequestHeal(int value)
    {
        if (!IsServer)
        {
            return;
        }
        _health.Value = Mathf.Min(Health + value, _definition.MaxHealth);
        InvokeHealedClientRpc();
    }
    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeHealedClientRpc()
    {
        Healed?.Invoke();
    }

    public void RequestKill()
    {
        if(!IsServer)
        {
            return;
        }
        _health.Value = 0;
        InvokeDiedClientRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeDiedClientRpc()
    {
        Died?.Invoke();
    }

    private void OnNetworkHealthValueChanged(int oldValue, int newValue)
    {
        HealthChanged?.Invoke(NormalizedHealth, Health);
    }

    #endregion

    #region Armor

    public bool TryEquipArmor(ArmorDefinition armorDefinition, ArmorBehavior armorBehavior)
    {
        if (!IsServer)
        {
            return false;
        }
        var equipped = _armorManager.TryEquipArmor(armorDefinition, armorBehavior);
        if (equipped)
        {
            InvokeArmorEquippedClientRpc(armorDefinition.Id);
        }
        return equipped;
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeArmorEquippedClientRpc(int armorDefinitionId)
    {
        ArmorEquipped?.Invoke(GameServices.ItemDatabase.GetById(armorDefinitionId) as ArmorDefinition);
    }

    public bool CanEquipArmor(ArmorDefinition definition)
    {
        if(!IsServer)
        {
            return false;
        }
        return _armorManager.CanEquip(definition);
    }

    private void InvokeArmorUnequipped(ArmorDefinition armor)
    {
        if(!IsServer)
        {
            return;
        }
        InvokeArmorUnequippedClientRpc(armor.Id);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeArmorUnequippedClientRpc(int armorDefinitionId)
    {
        ArmorUnequipped?.Invoke(GameServices.ItemDatabase.GetById(armorDefinitionId) as ArmorDefinition);
    }

    #endregion

    #region Movement

    public void RequestJump(Vector2 jumpVector)
    {
        if (!IsServer)
        {
            return;
        }
        InvokeJumpedClientRpc(jumpVector);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeJumpedClientRpc(Vector2 jumpVector)
    {
        Jumped?.Invoke(jumpVector);
    }

    public void RequestPrepareToJump()
    {
        if (!IsServer)
        {
            return;
        }
        InvokePreparedToJumpClientRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokePreparedToJumpClientRpc()
    {
        PreparedToJump?.Invoke();
    }

    public void RequestChangeJumpAim(Vector2 jumpVector)
    {
        if (!IsServer)
        {
            return;
        }
        InvokeJumpAimChangedClientRpc(jumpVector);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeJumpAimChangedClientRpc(Vector2 jumpVector)
    {
        JumpAimChanged?.Invoke(jumpVector);
    }

    public void RequestCancelJump()
    {
        if (!IsServer)
        {
            return;
        }
        InvokeJumpCancelledClientRpc();
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeJumpCancelledClientRpc()
    {
        JumpCancelled?.Invoke();
    }

    public void RequestPush(Vector2 pushVector)
    {
        if (!IsServer)
        {
            return;
        }
        InvokePushedClientRpc(pushVector);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokePushedClientRpc(Vector2 pushVector)
    {
        Pushed?.Invoke(pushVector);
    }

    public void RequestApplyJumpBoost(float jumpBoost)
    {
        if (!IsServer)
        {
            return;
        }
        _jumpBoost.Value = jumpBoost;
    }
    public void RequestRemoveJumpBoost()
    {
        if (!IsServer)
        {
            return;
        }
        _jumpBoost.Value = 0;
    }

    private void OnGroundCheckerIsGroundedChanged(bool isGrounded)
    {
        if(!IsServer)
        {
            return;
        }
        _isGrounded.Value = isGrounded;
    }

    private void OnNetworkIsGroundedValueChanged(bool oldValue, bool newValue)
    {
        IsGroundedChanged?.Invoke(newValue);
    }


    #endregion

    #region Aim

    public void RequestStartAim()
    {
        if (!IsServer)
        {
            return;
        }
        InvokeAimStartedClientRpc(SelectedItem.InstanceId);
    }
    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeAimStartedClientRpc(int selectedItemInstanceId)
    {
        var selectedItem = _inventory.GetItemByInstanceId(selectedItemInstanceId);
        AimStarted?.Invoke(selectedItem);
    }

    public void RequestChangeAim(Vector2 jumpVector)
    {
        if (!IsServer)
        {
            return;
        }
        InvokeAimChangedClientRpc(jumpVector);
    }
    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeAimChangedClientRpc(Vector2 jumpVector)
    {
        AimChanged?.Invoke(jumpVector);
    }

    public void RequestCancelAiming()
    {
        if (!IsServer)
        {
            return;
        }
        InvokeAimCancelledClientRpc();
    }
    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeAimCancelledClientRpc()
    {
        AimCancelled?.Invoke();
    }

    #endregion

    #region Items

    public void RequestAddItem(ItemInstance item)
    {
        if (!IsServer)
        {
            return;
        }
        _inventory.AddItem(item);
        item.QuantityChanged += OnItemQuantityChanged;
        AddItemClientRpc(new NetworkItemInstanceData(item));
    }


    [Rpc(SendTo.NotServer, InvokePermission = RpcInvokePermission.Server)]
    private void AddItemClientRpc(NetworkItemInstanceData itemInstanceData)
    {
        _inventory.AddItem(itemInstanceData.ToItemInstance());
    }

    private void OnItemQuantityChanged(ItemInstance itemInstance)
    {
        if(!IsServer)
        {
            return;
        }
        UpdateItemClientRpc(new NetworkItemInstanceData(itemInstance));
    }

    [Rpc(SendTo.NotServer, InvokePermission = RpcInvokePermission.Server)]
    private void UpdateItemClientRpc(NetworkItemInstanceData itemInstanceData)
    {
        _inventory.UpdateItem(itemInstanceData.ToItemInstance());
    }
    public void RequestRemoveItem(ItemInstance item)
    {
        if (!IsServer)
        {
            return;
        }
        RemoveItemClientRpc(item.InstanceId);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void RemoveItemClientRpc(int itemInstanceId)
    {
        _inventory.RemoveItem(itemInstanceId);
    }

    public void RequestSelectItem(ItemInstance item)
    {
        if (!IsServer)
        {
            return;
        }
        SelectItemClientRpc(item.InstanceId);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void SelectItemClientRpc(int itemInstanceId)
    {
        _inventory.SelectItem(itemInstanceId);
        ItemSelected?.Invoke(SelectedItem);
    }
    public void RequestUseSelectedItem(ItemUsageContext context)
    {
        if (!IsServer)
        {
            return;
        }
        var selectedItem = SelectedItem;
        SelectedItem.Use(context);
        _itemUsageState.Value = NetworkItemUsageState.CreateUsageStartedState(selectedItem, context);
        StartCoroutine(ChangeStateOnItemUsageFinished(selectedItem));
    }
    private IEnumerator ChangeStateOnItemUsageFinished(ItemInstance item)
    {
        yield return new WaitWhile(() => item.Behavior.IsInUse);
        _itemUsageState.Value = NetworkItemUsageState.CreateUsageFinishedState();
    }

    private void OnItemUsageStateChanged(NetworkItemUsageState previous, NetworkItemUsageState current)
    {
        if(current.IsInUse)
        {
            ItemUsed?.Invoke(_inventory.GetItemByInstanceId(current.ItemInstanceId), current.UsageContext.ToItemUsageContext(_character));
        }
    }

    public IEnumerable<ItemInstance> GetAllItems()
    {
        return _inventory.GetAllItems();
    }

    public ItemInstance GetItemByInstanceId(int instanceId)
    {
        return _inventory.GetItemByInstanceId(instanceId);
    }
    public void RequestCreateInitialItems()
    {
        if (!IsServer)
        {
            return;
        }
        foreach (var itemDef in _definition.InitialItems)
        {
            var instance = ItemInstance.CreateAsInitialItem(itemDef);
            RequestAddItem(instance);
        }
    }

    #endregion
}
