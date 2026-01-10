using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineCharacterState : NetworkBehaviour, ICharacterState
{
    public int Health => throw new NotImplementedException();

    public float NormalizedHealth => throw new NotImplementedException();

    public bool IsAlive => throw new NotImplementedException();

    public bool IsUsingSelectedItem => throw new NotImplementedException();

    public ItemInstance SelectedItem => throw new NotImplementedException();

    public float JumpBoost => throw new NotImplementedException();

    public float JumpStrength => throw new NotImplementedException();

    public Team Team => throw new NotImplementedException();

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

    public bool CanEquipArmor(ArmorDefinition definition)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ItemInstance> GetAllItems()
    {
        throw new NotImplementedException();
    }

    public void Initialize(CharacterDefinition characterDefinition, Team team)
    {
        throw new NotImplementedException();
    }

    public void RequestAddItem(ItemInstance item)
    {
        throw new NotImplementedException();
    }

    public void RequestApplyJumpBoost(float jumpBoost)
    {
        throw new NotImplementedException();
    }

    public void RequestHeal(int value)
    {
        throw new NotImplementedException();
    }

    public void RequestJump(Vector2 jumpVector)
    {
        throw new NotImplementedException();
    }

    public void RequestKill()
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

    public void RequestRemoveJumpBoost()
    {
        throw new NotImplementedException();
    }

    public void RequestSelectItem(ItemInstance item)
    {
        throw new NotImplementedException();
    }

    public void RequestTakeDamage(IDamageSourceDefinition weapon, int damageValue)
    {
        throw new NotImplementedException();
    }

    public void RequestUseItem(ItemInstance item, ItemUsageContext context)
    {
        throw new NotImplementedException();
    }

    public bool TryEquipArmor(ArmorDefinition armorDefinition, ArmorBehavior armorBehavior)
    {
        throw new NotImplementedException();
    }
}
