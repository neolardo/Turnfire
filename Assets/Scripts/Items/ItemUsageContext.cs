using UnityEngine;

public readonly struct ItemUsageContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Character Owner;
    public readonly ProjectilePool ProjectilePool;

    public ItemUsageContext(Vector2 aimOrigin, Vector2 aimVector, Character owner, ProjectilePool projectilePool)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        Owner = owner;
        ProjectilePool = projectilePool;
    }

    public ItemUsageContext(Character owner)
    {
        AimOrigin = default;
        AimVector = default;
        Owner = owner;
        ProjectilePool = default;
    }
}
