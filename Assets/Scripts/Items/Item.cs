using System;

public class Item : ICollectible
{
    public ItemDefinition Definition { get; private set; }
    public IItemBehavior Behavior { get; private set;}
    public int Quantity { get; private set; }

    public event Action<ICollectible> CollectibleDestroyed;

    public Item(ItemDefinition definition, bool initializeAsDrop = true)
    {
        Definition = definition;
        Behavior = definition.CreateItemBehavior();
        Quantity = initializeAsDrop ? definition.DropQuantityRange.CalculateValue() : definition.InitialQuantity;
        Behavior.ItemUsageFinished += OnItemUsageFinished;
    }
    private void OnItemUsageFinished()
    {
        DecreaseQuantity();
    }

    private void DecreaseQuantity()
    {
        Quantity--;
        if(Quantity == 0)
        {
            CollectibleDestroyed?.Invoke(this);
        }
    }

    public bool TryMerge(Item other)
    {
        if(!other.IsSameType(this))
        {
            return false;
        }
        if(Quantity < Definition.MaximumQuantity)
        {
            Quantity = Math.Min(Definition.MaximumQuantity, Quantity + other.Quantity);
            return true;
        }
        return false;
    }
   
    public bool IsSameType(Item item)
    {
        return item.Definition.Name == this.Definition.Name;
    }

    public bool TryCollect(Character c)
    {
        return c.TryAddItem(this);
    }
}
