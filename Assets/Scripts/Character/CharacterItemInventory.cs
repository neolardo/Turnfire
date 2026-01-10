using System;
using System.Collections.Generic;
using System.Linq;

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
    public void RemoveItem(int instanceId)
    {
        var item = GetItemByInstanceId(instanceId);
        ItemRemoved?.Invoke(item);
    }
    public void SelectItem(ItemInstance item)
    {
        SelectedItem = item;
        ItemSelected?.Invoke(item);
    }
    public void SelectItem(int instanceId)
    {
        var item = GetItemByInstanceId(instanceId);
        SelectedItem = item;
        ItemSelected?.Invoke(item);
    }

    private ItemInstance GetItemByInstanceId(int instanceId)
    {
        return _items.First(i => i.InstanceId == instanceId); //TODO: dict?
    }
    public IEnumerable<ItemInstance> GetAllItems()
    {
        return _items;
    }

}
