using UnityEngine;

public readonly struct ProjectileContactContext
{
    public readonly Projectile Projectile;
    public readonly Vector2 ContactPoint;

    public ProjectileContactContext(Projectile projectile, Vector2 contactPoint)
    {
        Projectile = projectile;
        ContactPoint = contactPoint;
    }
}
