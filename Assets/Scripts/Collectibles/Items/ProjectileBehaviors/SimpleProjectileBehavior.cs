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
        PlaceProjectile(context);
        rb.AddForce(context.AimVector, ForceMode2D.Impulse);
    }

    protected virtual void PlaceProjectile(ProjectileLaunchContext context)
    {
        var rb = context.ProjectileRigidbody;
        var targetPos = context.AimOrigin + context.AimVector.normalized * Constants.ProjectileOffset;
        rb.MovePosition(targetPos);
        rb.transform.position = targetPos;
        if (IsAnyColliderAtLaunchPoint(context, out var tag, out var contactPoint))
        {
            OnContact(new ProjectileContactContext(contactPoint, tag));
        }
    }

    protected bool IsAnyColliderAtLaunchPoint(ProjectileLaunchContext context, out string tag, out Vector2 point)
    {
        tag = null;
        point = Vector2.zero;

        int hitCount = Physics2D.RaycastNonAlloc(
            context.AimOrigin,
            context.AimVector.normalized,
            _raycastHits,
            Constants.ProjectileOffset,
            LayerMaskHelper.GetCombinedLayerMask(Constants.ProjectileCollisionLayers)
        );

        if (hitCount == 0)
        {
            return false;
        }

        Array.Sort(_raycastHits, 0, hitCount, RaycastHit2DComparer.Instance);

        for (int i = 0; i < hitCount; i++)
        {
            var hit = _raycastHits[i];
            if (hit.collider != null && hit.collider != context.OwnerCollider)
            {
                point = hit.point;
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

    public virtual Vector2 SimulateProjectileBehaviorAndCalculateClosestPositionToTarget(Vector2 start, Vector2 target, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner)
    {
        Vector2 velocity = aimVector;
        float minDist = Vector2.Distance(start, target);
        Vector2 minPos = start;
        Vector2 pos = start;
        const float dt = Constants.ParabolicPathSimulationDeltaForProjectiles;

        for (float t = 0; t < Constants.MaxParabolicPathSimulationTime; t += Constants.ParabolicPathSimulationDeltaForProjectiles)
        {
            pos += velocity * dt;
            velocity += Physics2D.gravity * dt;

            float currentDist = Vector2.Distance(pos, target);
            if (currentDist < minDist)
            {
                minDist = currentDist;
                minPos = pos;
            }

            if (!terrain.IsPointInsideBounds(pos) || terrain.OverlapPoint(pos))
            {
                break;
            }
        }

        return minPos;
    }
}
