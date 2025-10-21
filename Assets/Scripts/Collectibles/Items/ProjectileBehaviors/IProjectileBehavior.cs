using System;

public interface IProjectileBehavior
{
    public event Action<ExplosionInfo> Exploded;
    public void Launch(ProjectileLaunchContext context);
    public void OnContact(ProjectileContactContext context);
    public void SetProjectile(Projectile projectile);
}
