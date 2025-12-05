using UnityEngine;

public class SpriteRendererPool : Pool<SpriteRenderer>
{
    public override SpriteRenderer Get()
    {
        var item = base.Get();
        item.sortingOrder = _prefab.sortingOrder + TotalCount-1;
        return item;
    }
}
