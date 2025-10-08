public class Item : ICollectible
{
    public ItemDefinition Definition { get; private set; }
    public IItemBehavior Behavior { get; private set;}

    public Item(ItemDefinition definition)
    {
        Definition = definition;
        Behavior = definition.CreateItemBehavior(); //TODO: refactor?
    }

    public bool TryCollect(Character c)
    {
        return c.TryAddItem(this);
    }

    public bool IsSameType(Item item)
    {
        return item.Definition.Name == this.Definition.Name;
    }
}
