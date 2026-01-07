using System;
using System.Collections.Generic;

public interface ICharacterItemsController
{
    public bool IsUsingSelectedItem { get; }
    public float JumpBoost { get; }
    public Item SelectedItem { get; }

    public event Action<Item> SelectedItemChanged;
    public event Action<Item, ItemUsageContext> SelectedItemUsed;

    public void Initialize(CharacterDefinition characterDefinition);

    public void ApplyJumpBoost(float jumpBoost);
    public void RemoveJumpBoost();

    public bool TryAddItem(Item item);
    public IEnumerable<Item> GetAllItems();
    public void UseSelectedItem(ItemUsageContext context);
    public bool TrySelectItem(Item item, ItemUsageContext usageContext = default);
}
