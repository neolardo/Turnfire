using UnityEngine;

public readonly struct ProjectileLaunchContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Rigidbody2D ProjectileRigidbody;
    public readonly Collider2D ProjectileCollider;

    public ProjectileLaunchContext(Vector2 aimOrigin, Vector2 aimVector, Rigidbody2D projectileRigidbody, Collider2D projectileCollider)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        ProjectileRigidbody = projectileRigidbody;
        ProjectileCollider = projectileCollider;
    }

    public ProjectileLaunchContext(ItemUsageContext itemUsageContext, float aimMultiplier, Rigidbody2D projectileRigidbody, Collider2D projectileCollider)
    {
        AimOrigin = itemUsageContext.AimOrigin;
        AimVector = itemUsageContext.AimVector * aimMultiplier;
        ProjectileRigidbody = projectileRigidbody;
        ProjectileCollider = projectileCollider;
    }
}
