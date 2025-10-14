using UnityEngine;

public readonly struct ProjectileContactContext
{
    public readonly Projectile Projectile;
    public readonly Vector2 ContactPoint;
    public readonly ExplosionManager ExplosionManager; //TODO: refactor explosions

    public ProjectileContactContext(Projectile projectile, Vector2 contactPoint, ExplosionManager explosionManager)
    {
        Projectile = projectile;
        ContactPoint = contactPoint;
        ExplosionManager = explosionManager;
    }
}
