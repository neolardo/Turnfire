using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class OnineCharacterMovementController : NetworkBehaviour, ICharacterMovementController
{
    private Rigidbody2D _rb;
    public Collider2D Collider { get; private set; }
    public bool IsMoving => _rb.linearVelocity.magnitude > Mathf.Epsilon;
    public Vector2 FeetPosition => (Vector2)transform.position + Vector2.down * Collider.bounds.extents.y;
    public Vector2 FeetOffset => Vector2.down * Collider.bounds.extents.y;

    public event Action<Vector2> Jumped;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //TODO: use network transform
        _rb.simulated = IsServer;
    }

    #region Movement

    public void Push(Vector2 impulse)
    {
        if (IsServer)
        {
            ApplyPush(impulse);
        }
        else if (IsOwner)
        {
            PushServerRpc(impulse);
        }
    }

    public void StartJump(Vector2 jumpVector)
    {
        if (IsServer)
        {
            ApplyJump(jumpVector);
        }
        else if (IsOwner)
        {
            StartJumpServerRpc(jumpVector);
        }
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
        Jumped?.Invoke(jumpVector);
    }

    #endregion

    #region RPCs

    [Rpc(SendTo.Server, InvokePermission =RpcInvokePermission.Owner)]
    private void PushServerRpc(Vector2 impulse)
    {
        ApplyPush(impulse);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void StartJumpServerRpc(Vector2 jumpVector)
    {
        ApplyJump(jumpVector);
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
