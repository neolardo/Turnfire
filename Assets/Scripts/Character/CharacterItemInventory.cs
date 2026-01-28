using System;
using System.Collections.Generic;

public class CharacterItemInventory
{
    private Dictionary<int, ItemInstance> _itemsDict;
    public ItemInstance SelectedItem { get; private set; }

    public event Action<ItemInstance> ItemAdded;
    public event Action<ItemInstance> ItemRemoved;
    public event Action<ItemInstance> ItemSelected;

    public CharacterItemInventory()
    {
        _itemsDict = new Dictionary<int,ItemInstance>();
    }

    public void AddItem(ItemInstance item)
    {
        _itemsDict.Add(item.InstanceId,item);
        ItemAdded?.Invoke(item);
    }
    public void RemoveItem(ItemInstance item)
    { 
        _itemsDict.Remove(item.InstanceId);
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
    public void UpdateItem(ItemInstance item)
    {
        _itemsDict[item.InstanceId] = item;
        if(SelectedItem != null && SelectedItem.InstanceId == item.InstanceId)
        {
            SelectedItem = item;
        }
    }
    public ItemInstance GetItemByInstanceId(int instanceId)
    {
        return _itemsDict[instanceId];
    }
    public IEnumerable<ItemInstance> GetAllItems()
    {
        return _itemsDict.Values;
    }

}
