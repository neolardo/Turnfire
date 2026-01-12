using System;
using System.Collections;
using UnityEngine;

public class GrenadeProjectileBehavior : BallisticProjectileBehavior
{
    private GrenadeProjectileDefinition _definition;
    private int _contactCount;

    public GrenadeProjectileBehavior(GrenadeProjectileDefinition definition) : base(definition)
    {
        _definition = definition;
    }


    public override void Launch(ProjectileLaunchContext context)
    {
        _contactCount = 0; 
        _exploded = false;
        InitializePhysics(context.Physics);
        PlaceProjectile(context);
        context.Physics.AddImpulse(context.AimVector);
        StartCoroutine(ExplodeAfterDelay(_definition.ExplosionDelaySeconds, context.Physics));
    }

    protected override void InitializePhysics(ProjectilePhysics physics)
    {
        base.InitializePhysics(physics);
        _currentPhysics.InitializeAsBouncy(_definition.GrenadePhysicsMaterial);
    }

    protected override void PlaceProjectile(ProjectileLaunchContext context)
    {
        if (SafeObjectPlacer.TryFindSafePosition(context.AimOrigin, context.AimVector.normalized, LayerMaskHelper.GetCombinedLayerMask(Constants.HitboxCollisionLayers), _definition.ColliderRadius, out var safePosition))
        {
            context.Physics.Move(safePosition);
        }
        else 
        {
            context.Physics.Move(context.AimOrigin);
        }
    }

    private IEnumerator ExplodeAfterDelay(float delay, ProjectilePhysics physics)
    {
        yield return new WaitForSeconds(delay);
        Explode(new HitboxContactContext(physics.Position, null));
    }

    public override void OnContact(HitboxContactContext context)
    {
        _contactCount++;
        if (_contactCount >= _definition.ExplosionContactThreshold || (context.Collider != null && context.Collider.tag == Constants.DeadZoneTag))
        {
            Explode(context);
        }
        else
        {
            InvokeContactedWithoutExplosionEvent(context);
        }
    }

    public override IEnumerator SimulateProjectileBehavior(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone)
    {
        Vector2 velocity = context.AimVector;
        Vector2 pos = context.Origin;

        if (SafeObjectPlacer.TryFindSafePosition(context.Origin, context.AimVector.normalized, LayerMaskHelper.GetCombinedLayerMask(Constants.HitboxCollisionLayers), _definition.ColliderRadius, out var safePosition))
        {
            pos = safePosition;
        }

        int contactCount = 0;
        const float dt = Constants.ParabolicPathSimulationDeltaForProjectiles;

        for (float t = 0; t < Constants.MaxParabolicPathSimulationTime; t += Constants.ParabolicPathSimulationDeltaForProjectiles)
        {
            pos += velocity * dt;
            velocity += Physics2D.gravity * dt;

            bool contacted = false;
            Vector2 normal = default;
            foreach (var cornerPoint in _colliderCornerPoints)
            {
                var cornerPos = pos + cornerPoint;
                
                if (context.Terrain.OverlapPoint(cornerPos))
                {
                    normal = context.Terrain.GetNearestNormalAtPoint(cornerPos);
                    contacted = true;
                }
                else
                {
                    foreach(var c in context.OtherCharacters)
                    {
                        if (c.OverlapPoint(cornerPos))
                        {
                            normal = c.NormalAtPoint(cornerPos);
                            contacted = true;
                            break;
                        }
                    }
                }
                if(contacted)
                {
                    contactCount++;
                    velocity = PhysicsMaterial2DHelpers.ApplyMaterialBounce(velocity, normal, _definition.GrenadePhysicsMaterial);
                    if (contactCount >= _definition.ExplosionContactThreshold)
                    {
                        onDone?.Invoke(SimulateExplosion(pos, context.Owner));
                        yield break;
                    }
                    break;
                }
            }

            if(t > _definition.ExplosionDelaySeconds)
            {
                break;
            }

            if (!context.Terrain.IsPointInsideBounds(pos))
            {
                break;
            }
        }

        onDone?.Invoke(SimulateExplosion(pos, context.Owner));
    }

}
