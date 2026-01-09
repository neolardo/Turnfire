using System;

public class ItemInstance 
{
    public int InstanceId { get; private set; }
    public int DefinitionId { get; private set; }

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
                QuantityChanged?.Invoke();
            }
        }
    }

    private ItemDefinition _definition;

    public event Action QuantityChanged;
    public event Action Destroyed;

    public ItemInstance(int instanceId, int definitionId, int initialQuantity)
    {
        InstanceId = instanceId;
        DefinitionId = definitionId;
        Quantity = initialQuantity;
        _definition = GameServices.ItemDatabase.GetById(instanceId);
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
        if (Quantity < _definition.MaximumQuantity && !_definition.IsQuantityInfinite)
        {
            Quantity = Math.Min(_definition.MaximumQuantity, Quantity + other.Quantity);
            return true;
        }
        return false;
    }

    public void DecreaseQuantity()
    {
        if (_definition.IsQuantityInfinite)
        {
            return;
        }
        Quantity--;
        if (Quantity == 0)
        {
            Destroyed?.Invoke();
        }
    }

    public void Destroy()
    {
        Destroyed?.Invoke();
    }
}
