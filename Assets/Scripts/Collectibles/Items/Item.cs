public class Item : Collectible
{
    public ItemData ItemData;

    public override bool TryCollect(Character c)
    {
        return c.TryAddItem(this);
    }

    public bool IsSameType(Item item)
    {
        return item.ItemData.Name == this.ItemData.Name;
    }
}
