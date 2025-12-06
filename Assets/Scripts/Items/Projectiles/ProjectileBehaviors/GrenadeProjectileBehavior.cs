using System.Collections;
using System.Collections.Generic;
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
        var rb = _projectile.Rigidbody;
        var col = _projectile.Collider;
        col.isTrigger = false;
        col.sharedMaterial = _definition.GrenadePhysicsMaterial;
        PlaceProjectile(context);
        rb.AddForce(context.AimVector, ForceMode2D.Impulse);
        StartCoroutine(ExplodeAfterDelay(_definition.ExplosionDelaySeconds));
    }

    protected override void PlaceProjectile(ProjectileLaunchContext context)
    {
        var rb = _projectile.Rigidbody;
        if (SafeObjectPlacer.TryFindSafePosition(context.AimOrigin, context.AimVector.normalized, LayerMaskHelper.GetCombinedLayerMask(Constants.HitboxCollisionLayers), _definition.ColliderRadius, out var safePosition))
        {
            rb.transform.position = safePosition;
        }
        else 
        {
            rb.transform.position = context.AimOrigin;
        }
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode(new HitboxContactContext(_projectile.transform.position, null));
    }

    public override void OnContact(HitboxContactContext context)
    {
        _contactCount++;
        AudioManager.Instance.PlaySFXAt(_definition.ContactSFX, context.ContactPoint);
        if (_contactCount >= _definition.ExplosionContactThreshold || (context.Collider != null && context.Collider.tag == Constants.DeadZoneTag))
        {
            Explode(context);
        }
    }

    public override WeaponBehaviorSimulationResult SimulateProjectileBehavior(Vector2 start, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner, IEnumerable<Character> others)
    {
        Vector2 velocity = aimVector;
        Vector2 pos = start;

        if (SafeObjectPlacer.TryFindSafePosition(start, aimVector.normalized, LayerMaskHelper.GetCombinedLayerMask(Constants.HitboxCollisionLayers), _definition.ColliderRadius, out var safePosition))
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
                
                if (terrain.OverlapPoint(cornerPos)) // terrain contact
                {
                    normal = terrain.GetNearestNormalAtPoint(cornerPos);
                    contacted = true;
                }
                else
                {
                    foreach(var c in others)
                    {
                        if (c.OverlapPoint(cornerPos)) // character contact
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
                        return SimulateExplosion(pos, owner);
                    }
                    break;
                }
            }

            if(t > _definition.ExplosionDelaySeconds)
            {
                break;
            }

            if (!terrain.IsPointInsideBounds(pos))
            {
                break;
            }
        }

        return SimulateExplosion(pos, owner);
    }



}
