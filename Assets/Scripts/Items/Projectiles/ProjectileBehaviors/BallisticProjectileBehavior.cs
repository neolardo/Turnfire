using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BallisticProjectileBehavior : UnityDriven, IProjectileBehavior
{
    public event Action<ExplosionInfo> Exploded;

    private ProjectileDefinition _definition;
    protected Projectile _projectile;
    protected RaycastHit2D[] _raycastHits;
    protected bool _explodeOnce = true;
    protected bool _exploded;

    // simulation
    protected readonly Vector2[] _colliderCornerPoints = new Vector2[4];
    private readonly Collider2D[] _overlapCheckColliders = new Collider2D[Constants.OverlapHitColliderNumMax];

    // bot evaluation
    protected Character _lastOwner;

    public BallisticProjectileBehavior(ProjectileDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        _raycastHits = new RaycastHit2D[Constants.RaycastHitColliderNumMax];
        CacheColliderCornerPoints();
    }

    private void CacheColliderCornerPoints()
    {
        var r = _definition.ColliderRadius;
        _colliderCornerPoints[0] = new Vector2(-r, 0);
        _colliderCornerPoints[1] = new Vector2(r, 0);
        _colliderCornerPoints[2] = new Vector2(0, r);
        _colliderCornerPoints[3] = new Vector2(0, -r);
    }

    public void SetProjectile(Projectile projectile)
    {
        _projectile = projectile;
    }

    public virtual void Launch(ProjectileLaunchContext context)
    {
        _exploded = false;
        _lastOwner = context.OwnerCollider.GetComponent<Character>();
        var rb = _projectile.Rigidbody;
        PlaceProjectile(context);
        rb.AddForce(context.AimVector, ForceMode2D.Impulse);
    }

    protected virtual void PlaceProjectile(ProjectileLaunchContext context)
    {
        var rb = _projectile.Rigidbody;
        var targetPos = context.AimOrigin + context.AimVector.normalized * Constants.ProjectileOffset;
        rb.MovePosition(targetPos);
        rb.transform.position = targetPos;
        if (IsAnyColliderAtLaunchPoint(context, out var collider, out var contactPoint))
        {
            OnContact(new HitboxContactContext(contactPoint, collider));
        }
    }

    protected bool IsAnyColliderAtLaunchPoint(ProjectileLaunchContext context, out Collider2D collider, out Vector2 point)
    {
        collider = null;
        point = Vector2.zero;

        int hitCount = Physics2D.RaycastNonAlloc(
            context.AimOrigin,
            context.AimVector.normalized,
            _raycastHits,
            Constants.ProjectileOffset,
            LayerMaskHelper.GetCombinedLayerMask(Constants.HitboxCollisionLayers)
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
                collider = hit.collider;
                return true;
            }
        }

        return false;
    }


    public virtual void OnContact(HitboxContactContext context)
    {
        Explode(context);
    }

    protected void InvokeExplodedEvent(ExplosionInfo ei)
    {
        Exploded?.Invoke(ei);
    }

    protected virtual void Explode(HitboxContactContext context)
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
        var expInfo = new ExplosionInfo(damage, explodedCharacters, _projectile, exp);
        AddExplosionStats(expInfo);
        Exploded?.Invoke(expInfo);
    }

    protected void AddExplosionStats(ExplosionInfo expInfo)
    {
        var data = BotEvaluationStatistics.GetData(_lastOwner.Team);
        float allyDamage = 0;
        float enemyDamage = 0;
        foreach( var c in expInfo.ExplodedCharacters)
        {
            if(c.Team == _lastOwner.Team)
            {
                allyDamage += expInfo.Damage;
            }
            else
            {
                enemyDamage += expInfo.Damage;
            }
        }
        data.TotalDamageDealtToAllies += allyDamage;
        data.TotalDamageDealtToEnemies += enemyDamage;
        if(!expInfo.ExplodedCharacters.Any())
        {
            data.TotalNonDamagingAttackCount++;
        }
    }


    public virtual void ForceExplode()
    {
        Explode(new HitboxContactContext(_projectile.transform.position, null));
    }

    #region Simulation

    public virtual IEnumerator SimulateProjectileBehavior(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone)
    {
        Vector2 velocity = context.AimVector;
        Vector2 pos = context.Origin;
        const float dt = Constants.ParabolicPathSimulationDeltaForProjectiles;

        for (float t = 0; t < Constants.MaxParabolicPathSimulationTime; t += Constants.ParabolicPathSimulationDeltaForProjectiles)
        {
            pos += velocity * dt;
            velocity += Physics2D.gravity * dt;

            if (!context.Terrain.IsPointInsideBounds(pos))
            {
                onDone?.Invoke(SimulateExplosion(pos, context.Owner));
                yield break;
            }

            foreach (var cornerPoint in _colliderCornerPoints)
            {
                var cornerPos = pos + cornerPoint;

                if (context.Terrain.OverlapPoint(cornerPos))
                {
                    onDone?.Invoke(SimulateExplosion(pos, context.Owner));
                    yield break;
                }

                foreach (var c in context.OtherCharacters)
                {
                    if (c.OverlapPoint(cornerPos))
                    {
                        onDone?.Invoke(SimulateExplosion(pos, context.Owner));
                        yield break;
                    }
                }
            }
        }

        onDone?.Invoke(SimulateExplosion(pos, context.Owner));
        yield break;
    }

    public virtual ItemBehaviorSimulationResult SimulateProjectileBehaviorFast(ItemBehaviorSimulationContext context)
    {
        Vector2 velocity = context.AimVector;
        Vector2 pos = context.Origin;
        const float dt = Constants.ParabolicPathSimulationDeltaForProjectiles;

        for (float t = 0; t < Constants.MaxParabolicPathSimulationTime; t += Constants.ParabolicPathSimulationDeltaForProjectiles)
        {
            pos += velocity * dt;
            velocity += Physics2D.gravity * dt;

            if (!context.Terrain.IsPointInsideBounds(pos))
            {
                return SimulateExplosion(pos, context.Owner);
            }

            foreach (var cornerPoint in _colliderCornerPoints)
            {
                var cornerPos = pos + cornerPoint;

                if (context.Terrain.OverlapPoint(cornerPos))
                {
                    return SimulateExplosion(pos, context.Owner);
                }

                foreach (var c in context.OtherCharacters)
                {
                    if (c.OverlapPoint(cornerPos))
                    {
                        return SimulateExplosion(pos, context.Owner);
                    }
                }
            }
        }

        return SimulateExplosion(pos, context.Owner);
    }

    protected ItemBehaviorSimulationResult SimulateExplosion(Vector2 position, Character owner)
    {
        float radius = _definition.ExplosionDefinition.Radius.AvarageValue;
        float damage = _definition.Damage.AvarageValue;
        float allyDamage = 0;
        float enemyDamage = 0;

        var mask = LayerMaskHelper.GetLayerMask(Constants.CharacterLayer);
        var filter = new ContactFilter2D();
        filter.SetLayerMask(mask);
        int numHits = Physics2D.OverlapCircle(position, radius, filter, _overlapCheckColliders);
        for (int i = 0; i < numHits; i++)
        {
            var hit = _overlapCheckColliders[i];
            if (hit.TryGetComponent(out Character character))
            {
                if(character.Team == owner.Team)
                {
                    allyDamage += damage;
                }
                else
                {
                    enemyDamage += damage;
                }
            }
        }
        return ItemBehaviorSimulationResult.Damage(position, enemyDamage, allyDamage);
    } 

    #endregion
}
