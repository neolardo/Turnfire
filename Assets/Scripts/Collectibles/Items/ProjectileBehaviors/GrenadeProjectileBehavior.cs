using System.Collections;
using UnityEngine;

public class GrenadeProjectileBehavior : SimpleProjectileBehavior
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
        var rb = context.ProjectileRigidbody;
        var col = context.ProjectileCollider;
        col.isTrigger = false;
        col.sharedMaterial = _definition.GrenadePhysicsMaterial;
        PlaceProjectile(context);
        rb.AddForce(context.AimVector, ForceMode2D.Impulse);
        StartCoroutine(ExplodeAfterDelay(_definition.ExplosionDelaySeconds));
    }

    protected override void PlaceProjectile(ProjectileLaunchContext context)
    {
        var rb = context.ProjectileRigidbody;
        if (SafeObjectPlacer.TryFindSafePosition(context.AimOrigin, context.AimVector.normalized, LayerMaskHelper.GetCombinedLayerMask(Constants.ProjectileCollisionLayers), context.ProjectileCollider.radius, out var safePosition))
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
        Explode(new ProjectileContactContext(_projectile.transform.position, string.Empty));
    }

    public override void OnContact(ProjectileContactContext context)
    {
        _contactCount++;
        AudioManager.Instance.PlaySFXAt(_definition.ContactSFX, context.ContactPoint);
        if (_contactCount >= _definition.ExplosionContactThreshold || context.ContactObjectTag == Constants.DeadZoneTag)
        {
            Explode(context);
        }
    }

    public override Vector2 SimulateProjectileBehaviorAndCalculateClosestPositionToTarget(Vector2 start, Vector2 target, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner)
    {
        Vector2 velocity = aimVector;
        float minDist = Vector2.Distance(start, target);
        Vector2 minPos = start;
        Vector2 pos = start;
        int contactCount = 0;
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

            if (terrain.OverlapPoint(pos))
            {
                contactCount++;

                var normal = terrain.GetNearestNormalAtPoint(pos);
                Debug.DrawRay(pos, normal, Color.blue, 10); //TODO: delete
                velocity = PhysicsMaterial2DHelpers.ApplyMaterialBounce(velocity, normal, _definition.GrenadePhysicsMaterial);
                if (contactCount >= _definition.ExplosionContactThreshold)
                {
                    break;
                }
            }

            if (!terrain.IsPointInsideBounds(pos))
            {
                break;
            }
        }

        return minPos;
    }

   

}
