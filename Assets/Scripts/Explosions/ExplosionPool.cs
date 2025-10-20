public class ExplosionPool : Pool<Explosion>
{
    protected override Explosion CreateInstance()
    {
        var p = base.CreateInstance();
        p.ExplosionFinished += OnExplosionFinished;
        return p;
    }

    private void OnExplosionFinished(Explosion ex)
    {
        Release(ex);
    }

}