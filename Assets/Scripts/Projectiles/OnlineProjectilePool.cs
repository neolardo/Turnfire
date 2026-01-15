using System.Collections.Generic;
using Unity.Netcode;

public class OnlineProjectilePool : OnlinePool<OnlineProjectile>, IPool<IProjectile>
{
    private void Start()
    {
        GameServices.Register(this);
    }
    protected override OnlineProjectile CreateInstance()
    {
        var p = base.CreateInstance();
        p.Exploded += OnProjectileExploded;
        return p;
    }

    protected override void CreateInitialItems()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        var container = FindFirstObjectByType<ProjectileContainer>();
        _container = container.transform;
        base.CreateInitialItems();
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

