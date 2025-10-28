using System;
using UnityEngine;

public class SimpleProjectileBehavior : UnityDriven, IProjectileBehavior
{
    public event Action<ExplosionInfo> Exploded;

    private ProjectileDefinition _definition;
    protected Projectile _projectile;
    protected RaycastHit2D[] _raycastHits;
    protected bool _explodeOnce = true;
    protected bool _exploded;

    public SimpleProjectileBehavior(ProjectileDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        _raycastHits = new RaycastHit2D[Constants.RaycastHitColliderNumMax];
    }

    public void SetProjectile(Projectile projectile)
    {
        _projectile = projectile;
    }

    public virtual void Launch(ProjectileLaunchContext context)
    {
        _exploded = false;
        var rb = context.ProjectileRigidbody;
        rb.linearVelocity = Vector2.zero;
        PlaceProjectile(context);
        rb.AddForce(context.AimVector, ForceMode2D.Impulse);
    }

    protected virtual void PlaceProjectile(ProjectileLaunchContext context)
    {
        var rb = context.ProjectileRigidbody;
        rb.transform.position = context.AimOrigin + context.AimVector.normalized * Constants.ProjectileOffset;
        if (IsAnyColliderAtLaunchPoint(context, out var tag))
        {
            OnContact(new ProjectileContactContext(context.ProjectileCollider.transform.position, tag));
        }
    }

    protected bool IsAnyColliderAtLaunchPoint(ProjectileLaunchContext context, out string tag)
    {
        tag = null;
        Physics2D.RaycastNonAlloc(context.AimOrigin, context.AimVector.normalized, _raycastHits, Constants.ProjectileOffset, LayerMaskHelper.GetCombinedLayerMask(Constants.ProjectileCollisionLayers));
        foreach(var hit in _raycastHits)
        {
            if (hit.collider != null && hit.collider != context.OwnerCollider)
            {
                tag = hit.collider.tag;
                return true;
            }
        }
        return false;
    }


    public virtual void OnContact(ProjectileContactContext context)
    {
        Explode(context);
    }

    protected void InvokeExplodedEvent(ExplosionInfo ei)
    {
        Exploded?.Invoke(ei);
    }

    protected virtual void Explode(ProjectileContactContext context)
    {
        if(_explodeOnce && _exploded)
        {
            return;
        }
        var damage = _definition.Damage.CalculateValue();
        var exp = _projectile.ExplosionPool.Get();
        exp.Initialize(_definition.ExplosionDefinition);
        var explodedCharacters = exp.Explode(context.ContactPoint, damage);
        _exploded = true;
        Exploded?.Invoke(new ExplosionInfo(explodedCharacters, _projectile, exp));
    }

    public virtual void ForceExplode()
    {
        Explode(new ProjectileContactContext(_projectile.transform.position, null));
    }

}
