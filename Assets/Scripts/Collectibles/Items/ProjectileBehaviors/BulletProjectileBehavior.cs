using System.Collections;
using UnityEngine;

public class BulletProjectileBehavior : SimpleProjectileBehavior
{
    private BulletProjectileDefinition _definition;

    public BulletProjectileBehavior(BulletProjectileDefinition definition) : base(definition)
    {
        _definition = definition;
    }

    public override void Launch(ProjectileLaunchContext context)
    {
        _exploded = false;
        var rb = context.ProjectileRigidbody;
        float angle = Mathf.Atan2(context.AimVector.y, context.AimVector.x) * Mathf.Rad2Deg;
        rb.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        PlaceProjectile(context);
        rb.gravityScale = 0;
        rb.linearVelocity = context.AimVector / rb.mass;
        StartCoroutine(ExplodeAtRaycastTarget(context));
    }

    private IEnumerator ExplodeAtRaycastTarget(ProjectileLaunchContext context)
    {
        var rb = context.ProjectileRigidbody;
        var hit = Physics2D.Raycast(rb.transform.position, context.AimVector.normalized, Constants.ProjectileRaycastDistance, LayerMaskHelper.GetCombinedLayerMask(Constants.ProjectileCollisionLayers));
        if(hit.collider == null)
        {
            yield break;
        }
        var dist = hit.distance;
        var lastDist = dist;
        while(dist <= lastDist)
        {
            lastDist = dist;
            dist = (hit.point - (Vector2) rb.transform.position).magnitude;
            yield return new WaitForFixedUpdate();
        }
        ForceExplode();
    }


}