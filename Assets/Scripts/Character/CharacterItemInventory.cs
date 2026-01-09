using System;
using System.Collections.Generic;

public class CharacterItemInventory
{
    private List<ItemInstance> _items;
    public ItemInstance SelectedItem { get; private set; }

    public event Action<ItemInstance> ItemAdded;
    public event Action<ItemInstance> ItemRemoved;
    public event Action<ItemInstance> ItemSelected;

    public CharacterItemInventory()
    {
        _items = new List<ItemInstance>();
    }

    public void AddItem(ItemInstance item)
    {
        _items.Add(item);
        ItemAdded?.Invoke(item);
    }
    public void RemoveItem(ItemInstance item)
    {
        _items.Remove(item);
        ItemRemoved?.Invoke(item);
    }
    public void SelectItem(ItemInstance item)
    {
        SelectedItem = item;
        ItemSelected?.Invoke(item);
    }
    public IEnumerable<ItemInstance> GetAllItems()
    {
        return _items;
    }

}
