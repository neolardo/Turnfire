using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class OfflineCharacterPhysics : MonoBehaviour, ICharacterPhysics
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
    private void Start()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.simulated = true;
    }

    #region Movement

    public void Push(Vector2 impulse)
    {
        _rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void Jump(Vector2 jumpVector)
    {
        Debug.Log($"jump applied with force: {jumpVector}");
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
        float linearHalfLength = Collider.bounds.extents.y - Collider.bounds.extents.x;
        if (Mathf.Abs(point.y - transform.position.y) < linearHalfLength)
        {
            return new Vector2(point.x - transform.position.x, 0).normalized;
        }
        else
        {
            return (point - (Vector2)transform.position).normalized;
        }
    }

    #endregion
}
