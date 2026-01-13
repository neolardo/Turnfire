using System.Collections.Generic;

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
        base.Release(item as OnlineExplosion);
    }

    private void OnExplosionFinished(IExplosion ex)
    {
        Release(ex as OnlineExplosion);
    }

}