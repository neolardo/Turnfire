using System.Collections.Generic;
using UnityEngine;

public class OfflineExplosionPool : OfflinePool<OfflineExplosion>, IPool<IExplosion>
{
    private void Start()
    {
        GameServices.Register(this);
    }
    protected override OfflineExplosion CreateInstance()
    {
        var p = base.CreateInstance();
        p.Exploded += OnExplosionFinished;
        return p;
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
        base.Release(item as OfflineExplosion);
    }

    private void OnExplosionFinished(IExplosion ex)
    {
        Release(ex as OfflineExplosion);
    }

}