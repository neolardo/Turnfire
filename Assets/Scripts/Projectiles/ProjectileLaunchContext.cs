using UnityEngine;

public readonly struct ProjectileLaunchContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Collider2D OwnerCollider;
    public readonly ProjectilePhysics Physics;

    public ProjectileLaunchContext(Vector2 aimOrigin, Vector2 aimVector, Collider2D ownerCollider, ProjectilePhysics physics)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        OwnerCollider = ownerCollider;
        Physics = physics;
    }

    public ProjectileLaunchContext(ItemUsageContext itemUsageContext, float aimMultiplier, ProjectilePhysics physics)
    {
        AimOrigin = itemUsageContext.AimOrigin;
        AimVector = itemUsageContext.AimVector * aimMultiplier;
        OwnerCollider = itemUsageContext.Owner.Collider;
        Physics = physics;
    }
}
