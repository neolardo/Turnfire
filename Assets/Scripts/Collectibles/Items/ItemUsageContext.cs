using UnityEngine;

public readonly struct ItemUsageContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Transform Owner;
    public readonly ProjectileManager ProjectileManager;

    public ItemUsageContext(Vector2 aimOrigin, Vector2 aimVector, Transform owner, ProjectileManager projectileManager)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        Owner = owner;
        ProjectileManager = projectileManager;
    }
}
