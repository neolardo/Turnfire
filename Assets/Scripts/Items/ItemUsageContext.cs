using UnityEngine;

public readonly struct ItemUsageContext
{
    public readonly Vector2 AimOrigin;
    public readonly Vector2 AimVector;
    public readonly Character Owner;

    public ItemUsageContext(Vector2 aimOrigin, Vector2 aimVector, Character owner)
    {
        AimOrigin = aimOrigin;
        AimVector = aimVector;
        Owner = owner;
    }
    public ItemUsageContext(Character owner)
    {
        AimOrigin = default;
        AimVector = default;
        Owner = owner;
    }
}
