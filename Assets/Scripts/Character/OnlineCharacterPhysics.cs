using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[RequireComponent(typeof(NetworkRigidbody2D), typeof(NetworkTransform))]
public class OnlineCharacterPhysics : NetworkBehaviour, ICharacterPhysics
{
    private Rigidbody2D _rb;
    public Collider2D Collider { get; private set; }
    public bool IsMoving => _rb.linearVelocity.magnitude > Mathf.Epsilon;
    public Vector2 FeetPosition => (Vector2)transform.position + Vector2.down * Collider.bounds.extents.y;
    public Vector2 FeetOffset => Vector2.down * Collider.bounds.extents.y;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
    }

    #region Movement

    public void Push(Vector2 impulse)
    {
        if (!IsServer)
        {
            return;
        }
        ApplyPush(impulse);
    }

    public void Jump(Vector2 jumpVector)
    {
        if (!IsServer)
        {
            return;
        }
        Debug.Log($"jump applied with force: {jumpVector}");
        ApplyJump(jumpVector);
    }

    #endregion

    #region Server Logic

    private void ApplyPush(Vector2 impulse)
    {
        _rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    private void ApplyJump(Vector2 jumpVector)
    {
        _rb.AddForce(jumpVector, ForceMode2D.Impulse);
    }

    #endregion

    #region Simulation Helpers

    public bool OverlapPoint(Vector2 point)
    {
        return Collider.bounds.Contains(point);
    }

    public Vector2 NormalAtPoint(Vector2 point)
    {
        float linearHalfLength =Collider.bounds.extents.y - Collider.bounds.extents.x;

        if (Mathf.Abs(point.y - transform.position.y) < linearHalfLength)
        {
            return new Vector2(point.x - transform.position.x, 0).normalized;
        }

        return (point - (Vector2)transform.position).normalized;
    }

    #endregion
}
