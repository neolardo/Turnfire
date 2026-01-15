using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterState
{
    public int Health { get; }
    public float NormalizedHealth { get; }
    public bool IsAlive { get; }
    public bool IsUsingSelectedItem { get; }
    public ItemInstance SelectedItem { get; }
    public float JumpBoost { get; }
    public float JumpStrength { get; }
    public bool IsGrounded { get; }
    public Team Team { get; }

    public event Action<float, int> HealthChanged;
    public event Action Healed;
    public event Action Died;
    public event Action<IDamageSourceDefinition> Hurt;
    public event Action<ArmorDefinition> Blocked;
    public event Action<Vector2> Jumped;
    public event Action<Vector2> Pushed;
    public event Action<bool> IsGroundedChanged;

    public event Action<ItemInstance> AimStarted;
    public event Action<Vector2> AimChanged;
    public event Action AimCancelled;

    public event Action PreparedToJump;
    public event Action<Vector2> JumpAimChanged;
    public event Action JumpCancelled;

    public event Action<ItemInstance, ItemUsageContext> ItemUsed;
    public event Action<ItemInstance> ItemSelected;

    public event Action<ArmorDefinition> ArmorEquipped;
    public event Action<ArmorDefinition> ArmorUnequipped;

    public void Initialize(Character character, CharacterDefinition characterDefinition, Team team);
    public void RequestTakeDamage(IDamageSourceDefinition weapon, int damageValue);
    public void RequestHeal(int value);
    public void RequestKill();
    public bool TryEquipArmor(ArmorDefinition armorDefinition, ArmorBehavior armorBehavior);
    public bool CanEquipArmor(ArmorDefinition definition);
    public void RequestJump(Vector2 jumpVector);
    public void RequestPrepareToJump();
    public void RequestChangeJumpAim(Vector2 jumpVector);
    public void RequestCancelJump();
    public void RequestPush(Vector2 pushVector);
    public void RequestApplyJumpBoost(float jumpBoost);
    public void RequestRemoveJumpBoost();
    public void RequestStartAim();
    public void RequestChangeAim(Vector2 jumpVector);
    public void RequestCancelAiming();
    public void RequestCreateInitialItems();
    public void RequestAddItem(ItemInstance item);
    public void RequestRemoveItem(ItemInstance item);
    public void RequestSelectItem(ItemInstance item);
    public void RequestUseSelectedItem(ItemUsageContext context);
    public IEnumerable<ItemInstance> GetAllItems();
    public ItemInstance GetItemByInstanceId(int itemInstanceId);
}
