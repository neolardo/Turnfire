using Mono.Cecil;
using System;
using System.Linq;
using UnityEngine;

public class CharacterLogic
{
    private Character _character;
    private ICharacterState _state;
    public CharacterLogic(Character character, ICharacterState state, CharacterDefinition definition)
    {
        _character = character;
        _state = state;
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

    public void InitializeAndSelectInitialItem()
    {
        var items = _state.GetAllItems();
        foreach(var item in items)
        {
            item.Destroyed += OnItemDestroyed;
        }
        TrySelectAnyWeapon();
    }

    private void TrySelectAnyWeapon()
    {
        var allItems = _state.GetAllItems();
        TrySelectItem(allItems.Where(i => i.Definition.ItemType == ItemType.Weapon).FirstOrDefault());
    }

    public bool TryAddItem(ItemInstance item)
    {
        var items = _state.GetAllItems();
        var existingItem = items.FirstOrDefault(i => i.IsSameType(item));
        if (existingItem == null)
        {
            _state.RequestAddItem(item);
            item.Destroyed += OnItemDestroyed;
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

    private void OnItemDestroyed(ItemInstance destroyedItem)
    {
        var items = _state.GetAllItems();
        var item = items.FirstOrDefault(i => i.InstanceId == destroyedItem.InstanceId);
        if (item != null)
        {
            RemoveItem(item);
        }
    }

    private void RemoveItem(ItemInstance item)
    {
        var items = _state.GetAllItems();
        bool itemWasSelected = _state.SelectedItem == item;
        _state.RequestRemoveItem(item);
        item.Destroyed -= OnItemDestroyed;
        if (itemWasSelected)
        {
            TrySelectAnyWeapon();
        }
    }

    public void UseSelectedItem(ItemUsageContext context)
    {
        _state.RequestUseSelectedItem(context);
    }

    public bool TrySelectItem(ItemInstance item) 
    {
        var items = _state.GetAllItems();
        if ((item == null) || (items.Contains(item) && item != _state.SelectedItem))
        {
            if (item != null && item.Definition.UseInstantlyWhenSelected)
            {
                var context = new ItemUsageContext(_character);
                if (item.Behavior.CanUseItem(context))
                {
                    _state.RequestSelectItem(item);
                    if(_state.SelectedItem == item)
                    {
                        UseSelectedItem(context);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

    public bool CanSelectItem(ItemInstance item)
    {
        var items = _state.GetAllItems();
        if ((item == null) || (items.Contains(item) && item != _state.SelectedItem))
        {
            if (item != null && item.Definition.UseInstantlyWhenSelected)
            {
                var context = new ItemUsageContext(_character);
                return item.Behavior.CanUseItem(context);
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    #endregion

}
