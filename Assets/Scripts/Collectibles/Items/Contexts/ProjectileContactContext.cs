using UnityEngine;

public readonly struct ProjectileContactContext
{
    public readonly Vector2 ContactPoint;
    public readonly string ContactObjectTag;

    public ProjectileContactContext(Vector2 contactPoint, string contactObjectTag)
    {
        ContactPoint = contactPoint;
        ContactObjectTag = contactObjectTag;
    }
}
