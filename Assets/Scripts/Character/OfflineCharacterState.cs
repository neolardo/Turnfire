using System;
using System.Collections.Generic;
using UnityEngine;

public class OfflineCharacterState : ICharacterState
{
    private CharacterArmorManager _armorManager;
    private CharacterDefinition _definition;
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

    private List<Item> _items;
    public bool IsUsingSelectedItem => SelectedItem == null ? false : SelectedItem.Behavior.IsInUse;
    public Item SelectedItem { get; private set; }
    public float JumpBoost { get; private set; }
    public float JumpStrength => CharacterDefinition.JumpStrength + JumpBoost;
    public Team Team { get; private set; }

    public event Action<float, int> HealthChanged;
    public event Action Died;
    public event Action<ArmorDefinition> Blocked;
    public event Action Hurt;
    public event Action Healed;
    public event Action<Vector2> Jumped;
    public event Action<Vector2> Pushed;
    public event Action<Item, ItemUsageContext> ItemUsed;
    public event Action<Item> ItemSwitched;

    public OfflineCharacterState(CharacterDefinition characterDefinition, Team team, CharacterArmorManager armorManager)
    {
        _items = new List<Item>(); //TODO: initial items

        Team = team;
        _definition = characterDefinition;
        _armorManager = armorManager;
    }

    #region Health

    public void Damage(int value)
    {
        if (_armorManager.IsProtected)
        {
            var armor = _armorManager.BlockAttack();
            Blocked?.Invoke(armor);
        }
        else
        {
            Hurt?.Invoke();
            Health = Mathf.Max(0, Health - value);
            if (!IsAlive)
            {
                Die();
            }
        }
    }

    public void Heal(int value)
    {
        Health = Mathf.Min(Health + value, _definition.MaxHealth);
        Healed?.Invoke();
    }

    public void Kill()
    {
        Damage(Health);
    }

    private void Die()
    {
        Died?.Invoke();
    } 

    #endregion

    #region Movement

    public void RequestJump(Vector2 jumpVector)
    {
        Jumped?.Invoke(jumpVector);
    }
    public void RequestPush(Vector2 pushVector)
    {
        Pushed?.Invoke(pushVector);
    } 

    public void ApplyJumpBoost(float jumpBoost)
    {
        JumpBoost = jumpBoost;
    }
    public void RemoveJumpBoost()
    {
        JumpBoost = 0;
    }

    #endregion

    #region Items
    public void RequestAddItem(Item item)
    {
        _items.Add(item);
    }
    public void RequestRemoveItem(Item item)
    {
        _items.Remove(item);
    }
    public void RequestSelectItem(Item item)
    {
        SelectedItem = item;
        ItemSwitched?.Invoke(item);
    }
    public void RequestUseItem(Item item, ItemUsageContext context)
    {
        SelectedItem.Behavior.Use(context);
        ItemUsed?.Invoke(item, context);
    }
    public IEnumerable<Item> GetAllItems()
    {
        return _items;
    } 

    #endregion

}
