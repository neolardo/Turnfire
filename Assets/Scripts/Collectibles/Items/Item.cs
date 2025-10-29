using System;

public class Item : ICollectible
{
    public ItemDefinition Definition { get; private set; }
    public IItemBehavior Behavior { get; private set;}
    public int Ammo { get; private set; }

    public event Action<ICollectible> CollectibleDestroyed;

    public Item(ItemDefinition definition, bool initializeAsDrop = true)
    {
        Definition = definition;
        Behavior = definition.CreateItemBehavior();
        Ammo = initializeAsDrop ? definition.DropAmmoRange.CalculateValue() : definition.InitialAmmo;
        Behavior.ItemUsageFinished += OnItemUsageFinished;
    }
    private void OnItemUsageFinished()
    {
        DecreaseAmmo();
    }

    private void DecreaseAmmo()
    {
        Ammo--;
        if(Ammo == 0)
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
        if(Ammo < Definition.MaximumAmmo)
        {
            Ammo = Math.Min(Definition.MaximumAmmo, Ammo + other.Ammo);
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
