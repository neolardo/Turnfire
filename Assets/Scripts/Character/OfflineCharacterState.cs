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
    public Team Team { get; private set; }

    public event Action<float, int> HealthChanged;
    public event Action Died;
    public event Action<ArmorDefinition> Blocked;
    public event Action Hurt;
    public event Action Healed;
    public event Action Jumped;
    public event Action Pushed;
    public event Action<Item, ItemUsageContext> ItemUsed;
    public event Action<Item> ItemSwitched;

    public OfflineCharacterState(CharacterDefinition characterDefinition, Team team, CharacterArmorManager armorManager)
    {
        _items = new List<Item>(); //TODO
        Team = team;
        _definition = characterDefinition;
        _armorManager = armorManager;
    }


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

    public void RequestJump()
    {
        Jumped?.Invoke();
    }
    public void RequestPush()
    {
        Pushed?.Invoke();
    }
    public void RequestItemSwitched(Item item)
    {
        ItemSwitched?.Invoke(item);
    }
    public void RequestItemUsage(Item item,ItemUsageContext context)
    {
        ItemUsed?.Invoke(item, context);
    }
    public IEnumerable<Item> GetAllItems()
    {
        return _items;
    }

}
