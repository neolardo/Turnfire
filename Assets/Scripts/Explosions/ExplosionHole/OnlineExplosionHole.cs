using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(NetworkTransform))]
public class OnlineExplosionHole : NetworkBehaviour, IExplosionHole
{
    private CircleCollider2D _collider;

    public void Place(Vector2 worldPos, float explosionRadius) //TODO: broadcast to all clients
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.radius = explosionRadius;
        transform.position = worldPos;
    }
}
