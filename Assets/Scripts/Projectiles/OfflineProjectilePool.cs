using System.Collections.Generic;

public class OfflineProjectilePool : OfflinePool<OfflineProjectile>, IPool<IProjectile>
{

    protected override OfflineProjectile CreateInstance()
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
    public void Release(IProjectile item)
    {
        base.Release(item as OfflineProjectile);
    }

    private void OnProjectileExploded(IProjectile p)
    {
        Release(p);
    }
}
