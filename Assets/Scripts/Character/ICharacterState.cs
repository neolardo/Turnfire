using System;
using System.Collections.Generic;

public interface ICharacterState 
{
    int Health { get; }
    float NormalizedHealth { get; }
    Team Team { get; }
    bool IsAlive { get; }
    public bool IsUsingSelectedItem { get; }
    public Item SelectedItem { get; }
    public float JumpBoost { get; }

    event Action<float, int> HealthChanged;
    event Action Died;
    event Action<ArmorDefinition> Blocked;
    event Action Hurt;
    event Action Healed;
    event Action Jumped;
    event Action Pushed;
    event Action<Item, ItemUsageContext> ItemUsed;
    event Action<Item> ItemSwitched;

    void Damage(int value);
    void Heal(int value);
    void Kill();
    void RequestJump();
    void RequestPush();
    void RequestItemSwitched(Item item);
    void RequestItemUsage(Item item, ItemUsageContext context);
    public IEnumerable<Item> GetAllItems();
}
