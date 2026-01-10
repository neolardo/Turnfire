using System.Collections.Generic;

public class OfflineExplosionPool : OfflinePool<OfflineExplosion>, IPool<IExplosion>
{
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

    public void Release(IExplosion item)
    {
        base.Release(item as OfflineExplosion);
    }

    private void OnExplosionFinished(IExplosion ex)
    {
        Release(ex as OfflineExplosion);
    }

}