using System;
using System.Collections.Generic;
using UnityEngine;

public class OfflineCharacterState : MonoBehaviour, ICharacterState
{
    private Character _character;
    private CharacterDefinition _definition;
    private CharacterItemInventory _inventory;
    private CharacterArmorManager _armorManager;
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
        private set
        {
            if (_health != value)
            {
                _health = value;
                HealthChanged?.Invoke(NormalizedHealth, Health);
            }
        }
    }
    public float NormalizedHealth => _health / (float)_definition.MaxHealth;

    public bool IsAlive => _health > 0;

    public bool IsUsingSelectedItem => SelectedItem == null ? false : SelectedItem.Behavior.IsInUse;
    public ItemInstance SelectedItem => _inventory.SelectedItem;
    public float JumpBoost { get; private set; }
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
        _inventory = new CharacterItemInventory();
        _armorManager = new CharacterArmorManager();
        _armorManager.ArmorUnequipped += InvokeArmorUnequipped;
        _health = _definition.MaxHealth;
        foreach (var itemDef in _definition.InitialItems)
        {
            var instance = ItemInstance.CreateAsInitialItem(itemDef);
            _inventory.AddItem(instance);
        }
    }

    #region Health

    public void RequestTakeDamage(IDamageSourceDefinition weapon, int damageValue)
    {
        if (_armorManager.IsProtected)
        {
            var armor = _armorManager.BlockAttack();
            Blocked?.Invoke(armor);
        }
        else
        {
            Hurt?.Invoke(weapon);
            Health = Mathf.Max(0, Health - damageValue);
            if (!IsAlive)
            {
                Die();
            }
        }
    }

    public void RequestHeal(int value)
    {
        Health = Mathf.Min(Health + value, _definition.MaxHealth);
        Healed?.Invoke();
    }

    public void RequestKill()
    {
        Health = 0;
        Die();
    }

    private void Die()
    {
        Died?.Invoke();
    }

    #endregion

    #region Armor

    public bool TryEquipArmor(ArmorDefinition armorDefinition, ArmorBehavior armorBehavior)
    {
        var equipped = _armorManager.TryEquipArmor(armorDefinition, armorBehavior);
        if (equipped)
        {
            ArmorEquipped?.Invoke(armorDefinition);
        }
        return equipped;
    }

    public bool CanEquipArmor(ArmorDefinition definition)
    {
        return _armorManager.CanEquip(definition);
    }

    private void InvokeArmorUnequipped(ArmorDefinition armor)
    {
        ArmorUnequipped?.Invoke(armor);
    }

    #endregion

    #region Movement

    public void RequestJump(Vector2 jumpVector)
    {
        Debug.Log("jumped");
        Jumped?.Invoke(jumpVector);
    }
    public void RequestPush(Vector2 pushVector)
    {
        Pushed?.Invoke(pushVector);
    }
    public void RequestApplyJumpBoost(float jumpBoost)
    {
        JumpBoost = jumpBoost;
    }
    public void RequestRemoveJumpBoost()
    {
        JumpBoost = 0;
    }

    #endregion

    #region Items

    public void RequestAddItem(ItemInstance item)
    {
        _inventory.AddItem(item);
    }
    public void RequestRemoveItem(ItemInstance item)
    {
        _inventory.RemoveItem(item);
    }
    public void RequestSelectItem(ItemInstance item)
    {
        _inventory.SelectItem(item);
        ItemSelected?.Invoke(item);
    }
    public void RequestUseSelectedItem(ItemUsageContext context)
    {
        SelectedItem.Use(context);
        ItemUsed?.Invoke(SelectedItem, context);
    }
    public IEnumerable<ItemInstance> GetAllItems()
    {
        return _inventory.GetAllItems();
    }

    public ItemInstance GetItemByInstanceId(int instanceId)
    {
        return _inventory.GetItemByInstanceId(instanceId);
    }

    #endregion

}
