using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ExplosionHole : MonoBehaviour, IExplosionHole
{
    private CircleCollider2D _collider;

    public void Place(Vector2 worldPos, float explosionRadius)
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.radius = explosionRadius;
        transform.position = worldPos;
    }
}
