public class ProjectilePool : Pool<Projectile>
{
    protected override Projectile CreateInstance()
    {
        var p = base.CreateInstance();
        p.Exploded += OnProjectileExploded;
        return p;
    }

    private void OnProjectileExploded(ExplosionInfo ei)
    {
        Release(ei.Source);
    }
}
