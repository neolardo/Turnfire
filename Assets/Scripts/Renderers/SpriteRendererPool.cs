using System.Linq;
using UnityEngine;

public class SpriteRendererPool : OfflinePool<PoolableSpriteRenderer>
{
    public override PoolableSpriteRenderer Get()
    {
        var item = base.Get();
        item.SpriteRenderer.sortingOrder = _prefab.SpriteRenderer.sortingOrder + TotalCount-1;
        return item;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return Get().SpriteRenderer;
    }

    public void Release(SpriteRenderer spriteRenderer)
    {
        var item = _inUse.First(i => i.SpriteRenderer == spriteRenderer);
        if (item == null)
        {
            item = _available.First(i => i.SpriteRenderer == spriteRenderer);
            if(item == null)
            {
                Debug.LogWarning($"Trying to release an item that is not managed by this pool {gameObject.name}");
            }
            else
            {
                Debug.LogWarning("Trying to release an item that is not currently used by the pool");
                Release(item);
            }
        }
        else
        {
            Release(item);
        }

    }
}
