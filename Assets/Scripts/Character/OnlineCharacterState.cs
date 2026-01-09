using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineCharacterState : NetworkBehaviour, ICharacterState
{
    public int Health => throw new NotImplementedException();

    public float NormalizedHealth => throw new NotImplementedException();

    public Team Team => throw new NotImplementedException();

    public bool IsAlive => throw new NotImplementedException();

    public bool IsUsingSelectedItem => throw new NotImplementedException();

    public ItemInstance SelectedItem => throw new NotImplementedException();

    public float JumpBoost => throw new NotImplementedException();

    public float JumpStrength => throw new NotImplementedException();

    public event Action<float, int> HealthChanged;
    public event Action Died;
    public event Action<ArmorDefinition> Blocked;
    public event Action Hurt;
    public event Action Healed;
    public event Action<Vector2> Jumped;
    public event Action<Vector2> Pushed;
    public event Action<ItemInstance, ItemUsageContext> ItemUsed;
    public event Action<ItemInstance> ItemSelected;

    public void ApplyJumpBoost(float jumpBoost)
    {
        throw new NotImplementedException();
    }

    public void Damage(int value)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ItemInstance> GetAllItems()
    {
        throw new NotImplementedException();
    }

    public void Heal(int value)
    {
        throw new NotImplementedException();
    }

    public void Initialize(CharacterDefinition characterDefinition, Team team, CharacterArmorManager armorManager)
    {
        throw new NotImplementedException();
    }

    public void Kill()
    {
        throw new NotImplementedException();
    }

    public void RemoveJumpBoost()
    {
        throw new NotImplementedException();
    }

    public void RequestAddItem(ItemInstance item)
    {
        throw new NotImplementedException();
    }

    public void RequestJump(Vector2 jumpVector)
    {
        throw new NotImplementedException();
    }

    public void RequestPush(Vector2 pushVector)
    {
        throw new NotImplementedException();
    }

    public void RequestRemoveItem(ItemInstance item)
    {
        throw new NotImplementedException();
    }

    public void RequestSelectItem(ItemInstance item)
    {
        throw new NotImplementedException();
    }

    public void RequestUseItem(ItemInstance item, ItemUsageContext context)
    {
        throw new NotImplementedException();
    }
}
