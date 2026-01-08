using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterLogic
{
    private ICharacterState _state;
    public CharacterLogic(ICharacterState state, CharacterDefinition definition)
    {
        _state = state;
        AddInitialItems(definition.InitialItems);
    }

    #region Movement

    public void Push(Vector2 pushVector)
    {
        _state.RequestPush(pushVector);
    }

    public void Jump(Vector2 aimVector)
    {
        _state.RequestJump(aimVector * _state.JumpStrength);
    }

    #endregion

    #region Items

    private void AddInitialItems(IEnumerable<ItemDefinition> initialItems)
    {
        foreach (var itemDefinition in initialItems)
        {
            TryAddItem(new Item(itemDefinition, false));
        }
        var allItems = _state.GetAllItems();
        TrySelectItem(allItems.Where(i => i.Definition.ItemType == ItemType.Weapon).FirstOrDefault());
    }

    public bool TryAddItem(Item item)
    {
        var items = _state.GetAllItems();
        var existingItem = items.FirstOrDefault(i => i.IsSameType(item));
        if (existingItem == null)
        {
            _state.RequestAddItem(item);
            item.CollectibleDestroyed += OnItemDestroyed;
            if (_state.SelectedItem == null && item.Definition.ItemType == ItemType.Weapon)
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
        var items = _state.GetAllItems();
        var item = items.FirstOrDefault(i => i == collectible);
        if (item != null)
        {
            RemoveItem(item);
        }
    }

    private void RemoveItem(Item item)
    {
        var items = _state.GetAllItems();
        _state.RequestRemoveItem(item);
        if (_state == item)
        {
            TrySelectItem(items.FirstOrDefault(i => i.Definition.ItemType == ItemType.Weapon));
        }
    }

    public void UseSelectedItem(ItemUsageContext context)
    {
        _state.RequestUseItem(_state.SelectedItem, context);
    }

    public bool TrySelectItem(Item item, ItemUsageContext usageContext = default) //TODO: usage context
    {
        var items = _state.GetAllItems();
        if ((item == null) || (items.Contains(item) && item != _state.SelectedItem))
        {
            if (item != null && item.Definition.UseInstantlyWhenSelected)
            {
                if (item.Behavior.CanUseItem(usageContext))
                {
                    _state.RequestSelectItem(item);
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
                _state.RequestSelectItem(item);
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

}
