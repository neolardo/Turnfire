using System;
using UnityEngine;

public class ProjectilePhysics
{
    private Rigidbody2D _rb;
    private CircleCollider2D _col;
    public float RigidbodyMass => _rb.mass;
    public Vector2 Position => _rb.position; 

    public event Action<HitboxContactContext> Contacted;

    public ProjectilePhysics( Rigidbody2D rb, CircleCollider2D col)
    {
        _rb = rb;
        _col = col;
    }

    public void Initialize(ProjectileDefinition definition)
    {
        _col.radius = definition.ColliderRadius;
        _col.isTrigger = true;
        _col.sharedMaterial = null;
        _col.enabled = true;
        _rb.simulated = true;
        _rb.gravityScale = 1;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0;
        _rb.Sleep();
        _rb.WakeUp();
    }

    public void InitializeAsBouncy(PhysicsMaterial2D physicsMaterial)
    {
        _col.isTrigger = false;
        _col.sharedMaterial = physicsMaterial;
    }

    public void ApplyRotation(float angle)
    {
        _rb.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Shoot(Vector2 aimVector)
    {
        _rb.gravityScale = 0;
        _rb.linearVelocity = aimVector / RigidbodyMass;
    }

    public void AddImpulse(Vector2 impulse)
    {
        _rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void Move(Vector2 targetPosition)
    {
        _rb.MovePosition(targetPosition);
        _rb.transform.position = targetPosition;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Constants.GroundTag) || collider.CompareTag(Constants.CharacterTag) || collider.CompareTag(Constants.DeadZoneTag))
        {
            Contacted?.Invoke(new HitboxContactContext(_rb.position, collider));
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Contacted?.Invoke(new HitboxContactContext(_rb.position, collision.collider));
    }

    public RaycastHit2D RaycastFromCurrentPosition(Vector2 aimDirection)
    {
        return Physics2D.Raycast(_rb.transform.position, aimDirection, Constants.ProjectileRaycastDistance, LayerMaskHelper.GetCombinedLayerMask(Constants.HitboxCollisionLayers));
    }

    public void Stop()
    {
        _rb.gravityScale = 1;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0;
        _col.enabled = false;
        _rb.simulated = false;
        _rb.Sleep();
    }
}
