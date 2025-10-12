using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 offset = new Vector2(0f, 0.1f);
    [SerializeField] private float raySpacing = 0.2f;
    [SerializeField] private float rayLength = 0.2f;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRays = true;

    private bool _isGrounded;

    public event Action<bool> IsGroundedChanged;

    public bool IsGrounded => _isGrounded;

    private void FixedUpdate()
    {
        bool groundedNow = CheckGrounded();

        if (groundedNow != _isGrounded)
        {
            _isGrounded = groundedNow;
            IsGroundedChanged?.Invoke(_isGrounded);
        }
    }

    private bool CheckGrounded()
    {
        Vector2 origin = (Vector2)transform.position + offset;

        // Cast three rays: center, left, right
        Vector2 leftOrigin = origin + Vector2.left * raySpacing;
        Vector2 rightOrigin = origin + Vector2.right * raySpacing;

        bool centerHit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
        bool leftHit = Physics2D.Raycast(leftOrigin, Vector2.down, rayLength, groundLayer);
        bool rightHit = Physics2D.Raycast(rightOrigin, Vector2.down, rayLength, groundLayer);

        if (drawDebugRays)
        {
            Color color = (centerHit || leftHit || rightHit) ? Color.green : Color.red;
            Debug.DrawRay(leftOrigin, Vector2.down * rayLength, color);
            Debug.DrawRay(origin, Vector2.down * rayLength, color);
            Debug.DrawRay(rightOrigin, Vector2.down * rayLength, color);
        }

        return centerHit || leftHit || rightHit;
    }
}
