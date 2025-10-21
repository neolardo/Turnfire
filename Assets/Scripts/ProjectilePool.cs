public class ProjectilePool : Pool<Projectile>
{
    private CameraController _cameraController;
    protected override void Awake()
    {
        base.Awake();
        _cameraController = FindFirstObjectByType<CameraController>();
    }
    protected override Projectile CreateInstance()
    {
        var p = base.CreateInstance();
        p.Exploded += OnProjectileExploded;
        return p;
    }

    public override Projectile Get()
    {
        var p = base.Get();
        _cameraController.SetProjectileTarget(p);
        return p;
    }

    private void OnProjectileExploded(ExplosionInfo ei)
    {
        Release(ei.Source);
    }
}
