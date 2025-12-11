using UnityEngine;

public readonly struct HitboxContactContext
{
    public readonly Vector2 ContactPoint;
    public readonly Collider2D Collider;

    public HitboxContactContext(Vector2 contactPoint, Collider2D collider)
    {
        ContactPoint = contactPoint;
        Collider = collider;
    }
}
