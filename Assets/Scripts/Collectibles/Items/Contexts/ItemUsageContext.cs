using UnityEngine;

public readonly struct ItemUsageContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Transform Owner;
    public readonly Collider2D OwnerCollider;
    public readonly ProjectilePool ProjectilePool;

    public ItemUsageContext(Vector2 aimOrigin, Vector2 aimVector, Transform owner, Collider2D ownerCollider, ProjectilePool projectilePool)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        Owner = owner;
        OwnerCollider = ownerCollider;
        ProjectilePool = projectilePool;
    }
}
