using UnityEngine;

public readonly struct ProjectileContactContext
{
    public readonly Projectile Projectile;
    public readonly Vector2 ContactPoint;
    public readonly ExplosionPool ExplosionPool;

    public ProjectileContactContext(Projectile projectile, Vector2 contactPoint, ExplosionPool explosionPool)
    {
        Projectile = projectile;
        ContactPoint = contactPoint;
        ExplosionPool = explosionPool;
    }
}
