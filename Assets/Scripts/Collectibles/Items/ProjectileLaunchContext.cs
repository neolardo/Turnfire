using UnityEngine;

public readonly struct ProjectileLaunchContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Rigidbody2D ProjectileRigidbody;

    public ProjectileLaunchContext(Vector2 aimOrigin, Vector2 aimVector, Rigidbody2D projectileRigidbody)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        ProjectileRigidbody = projectileRigidbody;
    }

    public ProjectileLaunchContext(ItemUsageContext itemUsageContext, float aimMultiplier, Rigidbody2D projectileRigidbody)
    {
        AimOrigin = itemUsageContext.AimOrigin;
        AimVector = itemUsageContext.AimVector * aimMultiplier;
        ProjectileRigidbody = projectileRigidbody;
    }
}
