using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class OnlineCharacterItemsController : NetworkBehaviour, ICharacterItemsController //TODO: needed?
{
    private CharacterDefinition _characterDefinition; 
    private List<Item> _items;
    public bool IsUsingSelectedItem => SelectedItem == null ? false : SelectedItem.Behavior.IsInUse;
    public Item SelectedItem { get; private set; }
    public float JumpBoost { get; private set; }

    public event Action<Item> SelectedItemChanged;
    public event Action<Item, ItemUsageContext> SelectedItemUsed;

    public void Initialize(CharacterDefinition characterDefinition)
    {
        _characterDefinition = characterDefinition;
        _items = new List<Item>();
        foreach (var itemDefinition in _characterDefinition.InitialItems)
        {
            TryAddItem(new Item(itemDefinition, false));
        }
        SelectedItem = _items.FirstOrDefault();
    }

    #region Movement

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

    public bool TryAddItem(Item item)
    {
        var existingItem = _items.FirstOrDefault(i => i.IsSameType(item));
        if (existingItem == null)
        {
            _items.Add(item);
            item.CollectibleDestroyed += OnItemDestroyed;
            if (SelectedItem == null && item.Definition.ItemType == ItemType.Weapon)
            {
                TrySelectItem(item);
            }
            return true;
        }
        else
        {
            return existingItem.TryMerge(item);
        }
    }

    private void OnItemDestroyed(ICollectible collectible)
    {
        var item = _items.FirstOrDefault(i => i == collectible);
        if (item != null)
        {
            RemoveItem(item);
        }
    }

    private void RemoveItem(Item item)
    {
        _items.Remove(item);
        if (SelectedItem == item)
        {
            TrySelectItem(_items.FirstOrDefault(i => i.Definition.ItemType == ItemType.Weapon));
        }
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _items;
    }

    #region Selected Item

    public void UseSelectedItem(ItemUsageContext context)
    {
        SelectedItem.Behavior.Use(context);
        SelectedItemUsed?.Invoke(SelectedItem, context);
    }

    public bool TrySelectItem(Item item, ItemUsageContext usageContext = default)
    {
        if ((item == null) || (_items.Contains(item) && item != SelectedItem))
        {
            if (item != null && item.Definition.UseInstantlyWhenSelected)
            {
                if (item.Behavior.CanUseItem(usageContext))
                {
                    SelectedItem = item;
                    SelectedItemChanged?.Invoke(item);
                    UseSelectedItem(usageContext);
                    // instantly used items should be deselected after usage
                    return TrySelectItem(null);
                }
                else
                {
                    Debug.LogWarning($"Item should be used instantly but was not able to use it: {item.Definition.Name}");
                    return false;
                }
            }
            else
            {
                SelectedItem = item;
                SelectedItemChanged?.Invoke(item);
                return true;
            }
        }
        else
        {
            Debug.LogWarning($"Item selection failed for item: {item.Definition.Name}.");
            return false;
        }
    }

    #endregion

    #endregion
}
