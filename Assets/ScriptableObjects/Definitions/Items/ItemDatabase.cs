using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/Items/ItemDatabase")]
public class ItemDatabase : ScriptableObject, IItemDatabase
{
    [SerializeField] private List<ItemDefinition> _items;
    private Dictionary<int, ItemDefinition> _itemsByIdDict;
    
    public void Initialize()
    {
        _itemsByIdDict = new Dictionary<int, ItemDefinition>();
        int id = 0;
        foreach (var itemDef in _items)
        {
            itemDef.AssingId(id);
            _itemsByIdDict.Add(itemDef.Id, itemDef);
            id++;
        }
    }

    public ItemDefinition GetById(int id)
    {
        return _itemsByIdDict[id];
    }
   
}
