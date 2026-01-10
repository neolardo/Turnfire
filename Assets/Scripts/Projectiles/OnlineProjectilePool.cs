using System.Collections.Generic;

public class OnlineProjectilePool : OnlinePool<OnlineProjectile>, IPool<IProjectile>
{
    protected override OnlineProjectile CreateInstance()
    {
        var p = base.CreateInstance();
        p.Exploded += OnProjectileExploded;
        return p;
    }

    IProjectile IPool<IProjectile>.Get()
    {
        return Get();
    }

    IEnumerable<IProjectile> IPool<IProjectile>.GetMultiple(int count)
    {
        return GetMultiple(count);
    }

    private void OnProjectileExploded(IProjectile p)
    {
        Release(p);
    }
    public void Release(IProjectile item)
    {
        base.Release(item as OnlineProjectile);
    }

}

