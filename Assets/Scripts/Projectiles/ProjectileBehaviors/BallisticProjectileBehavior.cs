using System;
using System.Collections;
using UnityEngine;

public class BallisticProjectileBehavior : UnityDriven, IProjectileBehavior
{
    private ProjectileDefinition _definition;
    protected ProjectilePhysics _currentPhysics;
    protected readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[Constants.RaycastHitColliderNumMax];
    protected bool _explodeOnce = true;
    protected bool _exploded;

    public event Action<ExplosionInfo> Exploded;
    public event Action<HitboxContactContext> ContactedWithoutExplosion;

    // simulation
    protected readonly Vector2[] _colliderCornerPoints = new Vector2[4];
    protected readonly Collider2D[] _overlapCheckColliders = new Collider2D[Constants.OverlapHitColliderNumMax];

    public BallisticProjectileBehavior(ProjectileDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
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

    public virtual void Launch(ProjectileLaunchContext context)
    {
        _exploded = false;
        InitializePhysics(context.Physics);
        PlaceProjectile(context);
        context.Physics.AddImpulse(context.AimVector);
    }

    protected virtual void InitializePhysics(ProjectilePhysics physics)
    {
        if(_currentPhysics != null)
        {
            _currentPhysics.Contacted -= OnContact;
        }
        _currentPhysics = physics;
        _currentPhysics.Contacted += OnContact;
    }

    protected virtual void PlaceProjectile(ProjectileLaunchContext context)
    {
        var targetPos = context.AimOrigin + context.AimVector.normalized * Constants.ProjectileOffset;
        context.Physics.Move(targetPos);
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
    protected void InvokeContactedWithoutExplosionEvent(HitboxContactContext context)
    {
        ContactedWithoutExplosion?.Invoke(context);
    }

    protected virtual void Explode(HitboxContactContext context)
    {
        if(_explodeOnce && _exploded)
        {
            return;
        }
        StartCoroutine(CreateExplosion(context));
    }

    protected IEnumerator CreateExplosion(HitboxContactContext context)
    {
        _exploded = true;
        var damage = _definition.Damage.CalculateValue();
        var exp = GameServices.ExplosionPool.Get();
        yield return new WaitUntil( () => exp.IsReady);
        exp.Initialize(_definition.ExplosionDefinition);
        var explodedCharacters = exp.Explode(context.ContactPoint, damage, _definition);
        var expInfo = new ExplosionInfo(explodedCharacters, exp);
        Exploded?.Invoke(expInfo);
    }

    public virtual void ForceExplode()
    {
        Explode(new HitboxContactContext(_currentPhysics.Position, null));
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

    protected ItemBehaviorSimulationResult SimulateExplosion(Vector2 position, Character owner)
    {
        float radius = _definition.ExplosionDefinition.Radius.AvarageValue;
        int damage = _definition.Damage.AvarageValue;
        int allyDamage = 0;
        int enemyDamage = 0;

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
