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
        TrySelectAnyWeapon();
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
        _state.RequestRemoveItem(item);
        if (_state.SelectedItem == item)
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
                    UseSelectedItem(context);
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
