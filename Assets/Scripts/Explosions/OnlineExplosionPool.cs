using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineExplosionPool : OnlinePool<OnlineExplosion>, IPool<IExplosion>
{
    private void Start()
    {
        GameServices.Register(this);
    }
    protected override OnlineExplosion CreateInstance()
    {
        var p = base.CreateInstance();
        p.Exploded += OnExplosionFinished;
        return p;
    }
    protected override void CreateInitialItems()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        var container = FindFirstObjectByType<ExplosionContainer>();
        _container = container.transform;
        base.CreateInitialItems();
    }

    IExplosion IPool<IExplosion>.Get()
    {
        return Get();
    }

    IEnumerable<IExplosion> IPool<IExplosion>.GetMultiple(int count)
    {
        return GetMultiple(count);
    }
    IExplosion IPool<IExplosion>.GetAndPlace(Vector2 position)
    {
        return GetAndPlace(position);
    }
    public void Release(IExplosion item)
    {
        base.Release(item as OnlineExplosion);
    }

    private void OnExplosionFinished(IExplosion ex)
    {
        Release(ex as OnlineExplosion);
    }

}