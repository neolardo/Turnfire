using UnityEngine;

public readonly struct ProjectileLaunchContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Rigidbody2D ProjectileRigidbody;
    public readonly CircleCollider2D ProjectileCollider;
    public readonly Collider2D OwnerCollider;

    public ProjectileLaunchContext(Vector2 aimOrigin, Vector2 aimVector, Rigidbody2D projectileRigidbody, CircleCollider2D projectileCollider, Collider2D ownerCollider)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        ProjectileRigidbody = projectileRigidbody;
        ProjectileCollider = projectileCollider;
        OwnerCollider = ownerCollider;
    }

    public ProjectileLaunchContext(ItemUsageContext itemUsageContext, float aimMultiplier, Rigidbody2D projectileRigidbody, CircleCollider2D projectileCollider)
    {
        AimOrigin = itemUsageContext.AimOrigin;
        AimVector = itemUsageContext.AimVector * aimMultiplier;
        ProjectileRigidbody = projectileRigidbody;
        ProjectileCollider = projectileCollider;
        OwnerCollider = itemUsageContext.OwnerCollider;
    }
}
