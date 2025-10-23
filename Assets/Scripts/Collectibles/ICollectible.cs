using System;

public interface ICollectible
{
    public event Action<ICollectible> CollectibleDestroyed;
    public bool TryCollect(Character c);
}
