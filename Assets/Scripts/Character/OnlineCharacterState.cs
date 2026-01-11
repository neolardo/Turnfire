using System;
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
    public int Health
    {
        get
        {
            return _health.Value;
        }
        private set
        {
            if (!IsServer)
            {
                return;
            }
            _health.Value = value;
        }
    }
    public float NormalizedHealth => Health / (float)_definition.MaxHealth;

    public bool IsAlive => Health > 0;

    public bool IsUsingSelectedItem => SelectedItem == null ? false : SelectedItem.Behavior.IsInUse;
    public ItemInstance SelectedItem { get; private set; }

    private NetworkVariable<float> _jumpBoost = new NetworkVariable<float>();
    public float JumpBoost
    {
        get 
        { 
            return _jumpBoost.Value; 
        }
        private set
        {
            if(!IsServer)
            { 
                return;
            }    
            _jumpBoost.Value = value;
        }
    }
    public float JumpStrength => CharacterDefinition.JumpStrength + JumpBoost;
    public Team Team { get; private set; }

    public event Action<float, int> HealthChanged;
    public event Action Healed;
    public event Action Died;
    public event Action<IDamageSourceDefinition> Hurt;
    public event Action<ArmorDefinition> Blocked;
    public event Action<Vector2> Jumped;
    public event Action<Vector2> Pushed;

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
        if (!IsServer)
        {
            return; 
        }

        _armorManager.ArmorUnequipped += InvokeArmorUnequipped;
        foreach (var itemDef in _definition.InitialItems)
        {
            _inventory.AddItem(ItemInstance.CreateAsInitialItem(itemDef));
        }
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
            Health = Mathf.Max(0, Health - damageValue);
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
        Health = Mathf.Min(Health + value, _definition.MaxHealth);
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
        Health = 0;
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
        JumpBoost = jumpBoost;
    }
    public void RequestRemoveJumpBoost()
    {
        if (!IsServer)
        {
            return;
        }
        JumpBoost = 0;
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
        SelectedItem.Behavior.Use(context);
        InvokeSelectedItemUsedClientRpc(new NetworkItemUsageContextData(context));
    }
    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void InvokeSelectedItemUsedClientRpc(NetworkItemUsageContextData networkContext)
    {
        ItemUsed?.Invoke(SelectedItem, networkContext.ToItemUsageContext(_character));
    }
    public IEnumerable<ItemInstance> GetAllItems()
    {
        return _inventory.GetAllItems();
    }

    #endregion
}
