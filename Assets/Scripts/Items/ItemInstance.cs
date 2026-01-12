using System;
using Unity.Netcode;

public class ItemInstance
{
    public int InstanceId { get;  }
    public int DefinitionId { get; }

    private int _quantity;
    public int Quantity
    {
        get 
        {
            return _quantity; 
        }
        set
        {
            if(_quantity != value) 
            {
                _quantity = value;
                QuantityChanged?.Invoke(this);
            }
        }
    }

    public IItemBehavior Behavior { get; }
    public ItemDefinition Definition { get; }

    public event Action<ItemInstance> QuantityChanged;
    public event Action<ItemInstance> Destroyed;

    public ItemInstance(int instanceId, int definitionId, int initialQuantity)
    {
        InstanceId = instanceId;
        DefinitionId = definitionId;
        Quantity = initialQuantity;
        Definition = GameServices.ItemDatabase.GetById(definitionId);
        Behavior = Definition.CreateItemBehavior();
    }

    public static ItemInstance CreateAsInitialItem(ItemDefinition definition)
    {
        return new ItemInstance(GameServices.ItemInstanceIdGenerator.GenerateId(), definition.Id, definition.InitialQuantity);
    }
    public static ItemInstance CreateAsDrop(ItemDefinition definition)
    {
        return new ItemInstance(GameServices.ItemInstanceIdGenerator.GenerateId(), definition.Id, definition.DropQuantityRange.CalculateValue());
    }

    public bool IsSameType(ItemInstance other)
    {
        return DefinitionId == other.DefinitionId;
    }

    public bool TryMerge(ItemInstance other)
    {
        if (!other.IsSameType(this))
        {
            return false;
        }
        if (Quantity < Definition.MaximumQuantity && !Definition.IsQuantityInfinite)
        {
            Quantity = Math.Min(Definition.MaximumQuantity, Quantity + other.Quantity);
            other.Destroy();
            return true;
        }
        return false;
    }

    public void DecreaseQuantity()
    {
        if (Definition.IsQuantityInfinite)
        {
            return;
        }
        Quantity--;
        if (Quantity == 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroyed?.Invoke(this);
    }
}
