using UnityEngine;

public readonly struct ProjectileLaunchContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Collider2D OwnerCollider;

    public ProjectileLaunchContext(Vector2 aimOrigin, Vector2 aimVector, Collider2D ownerCollider)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        OwnerCollider = ownerCollider;
    }

    public ProjectileLaunchContext(ItemUsageContext itemUsageContext, float aimMultiplier)
    {
        AimOrigin = itemUsageContext.AimOrigin;
        AimVector = itemUsageContext.AimVector * aimMultiplier;
        OwnerCollider = itemUsageContext.Owner.Collider;
    }
}
