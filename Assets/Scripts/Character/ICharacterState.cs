using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterState 
{
    int Health { get; }
    float NormalizedHealth { get; }
    Team Team { get; }
    bool IsAlive { get; }
    bool IsUsingSelectedItem { get; }
    Item SelectedItem { get; }
    float JumpBoost { get; }
    float JumpStrength { get; }

    event Action<float, int> HealthChanged;
    event Action Died;
    event Action<ArmorDefinition> Blocked;
    event Action Hurt;
    event Action Healed;
    event Action<Vector2> Jumped;
    event Action<Vector2> Pushed;
    event Action<Item, ItemUsageContext> ItemUsed;
    event Action<Item> ItemSwitched;

    void Damage(int value);
    void Heal(int value);
    void Kill();
    void RequestJump(Vector2 jumpVector);
    void RequestPush(Vector2 pushVector);
    void ApplyJumpBoost(float jumpBoost);
    void RemoveJumpBoost();
    void RequestAddItem(Item item);
    void RequestRemoveItem(Item item);
    void RequestSelectItem(Item item);
    void RequestUseItem(Item item, ItemUsageContext context);
    IEnumerable<Item> GetAllItems();
}
