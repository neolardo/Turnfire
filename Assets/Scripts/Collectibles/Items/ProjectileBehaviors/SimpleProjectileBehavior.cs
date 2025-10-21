using System;
using UnityEngine;
using UnityEngineInternal;

public class SimpleProjectileBehavior : UnityDriven, IProjectileBehavior
{
    public event Action<ExplosionInfo> Exploded;

    private ProjectileDefinition _definition;
    protected Projectile _projectile;
    protected RaycastHit2D[] _raycastHits;

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
        var rb = context.ProjectileRigidbody;
        rb.linearVelocity = Vector2.zero;
        rb.transform.position = context.AimOrigin + context.AimVector.normalized * Constants.ProjectileOffset;
        rb.AddForce(context.AimVector, ForceMode2D.Impulse);
        TryContactImmadiatelyOnLaunchIfNearAnyCollider(context);
    }

    protected bool TryContactImmadiatelyOnLaunchIfNearAnyCollider(ProjectileLaunchContext context)
    {
        Physics2D.RaycastNonAlloc(context.AimOrigin, context.AimVector.normalized, _raycastHits,Constants.ProjectileOffset, LayerMaskHelper.GetCombinedLayerMask(Constants.ProjectileCollisionLayers));
        foreach(var hit in _raycastHits)
        {
            if (hit.collider != null && hit.collider != context.OwnerCollider)
            {
                OnContact(new ProjectileContactContext(context.ProjectileCollider.transform.position, hit.collider.tag));
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
        var damage = _definition.Damage.CalculateValue();
        var exp = _projectile.ExplosionPool.Get();
        exp.Initialize(_definition.ExplosionDefinition);
        var explodedCharacters = exp.Explode(context.ContactPoint, damage);
        Exploded?.Invoke(new ExplosionInfo(explodedCharacters, _projectile, exp));
    }

}
