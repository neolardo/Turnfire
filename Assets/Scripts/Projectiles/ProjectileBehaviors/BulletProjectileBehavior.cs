using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BulletProjectileBehavior : BallisticProjectileBehavior
{
    private BulletProjectileDefinition _definition;

    public BulletProjectileBehavior(BulletProjectileDefinition definition) : base(definition)
    {
        _definition = definition;
    }

    public override void Launch(ProjectileLaunchContext context)
    {
        _exploded = false;
        InitializePhysics(context.Physics);
        float angle = Mathf.Atan2(context.AimVector.y, context.AimVector.x) * Mathf.Rad2Deg;
        context.Physics.ApplyRotation(angle);
        PlaceProjectile(context);
        context.Physics.Shoot(context.AimVector);
        StartCoroutine(ExplodeAtRaycastTarget(context));
    }

    private IEnumerator ExplodeAtRaycastTarget(ProjectileLaunchContext context)
    {
        var physics = context.Physics;
        var hit = physics.RaycastFromCurrentPosition(context.AimVector.normalized);
        if (hit.collider == null)
        {
            yield break;
        }
        var dist = hit.distance;
        var lastPoint = physics.Position;
        var lastDist = float.PositiveInfinity;
        while (lastDist >= dist && !_exploded)
        {
            yield return new WaitForFixedUpdate();
            lastDist = dist;
            dist = (hit.point - physics.Position).magnitude;
        }
        Explode(new HitboxContactContext(hit.point, hit.collider));
    }

    public override IEnumerator SimulateProjectileBehavior(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone)
    {
        var numHits = Physics2D.RaycastNonAlloc(context.Origin, context.AimVector, _raycastHits, Constants.ProjectileRaycastDistance, LayerMaskHelper.GetCombinedLayerMask(Constants.HitboxCollisionLayers));
        var closestHit = _raycastHits.Take(numHits).Where(hit => hit.collider != context.Owner.Collider).OrderBy(hit => hit.distance).FirstOrDefault();
        
        if (closestHit.collider == null)
        {
            onDone?.Invoke(ItemBehaviorSimulationResult.None);
        }
        else
        {
            onDone?.Invoke(SimulateExplosion(closestHit.point, context.Owner));
        }
        yield return null;
    }
}