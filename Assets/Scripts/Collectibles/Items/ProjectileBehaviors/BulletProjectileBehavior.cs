using System.Collections;
using System.Linq;
using UnityEngine;

public class BulletProjectileBehavior : SimpleProjectileBehavior
{
    private BulletProjectileDefinition _definition;

    private readonly RaycastHit2D[] raycastHitArray = new RaycastHit2D[Constants.RaycastHitColliderNumMax];

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
        rb.gravityScale = 0;
        PlaceProjectile(context);
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
        var nextPoint = (Vector2)rb.transform.position + rb.linearVelocity * Time.fixedDeltaTime;
        var nextDist = (hit.point - nextPoint).magnitude;
        while(nextDist <= dist && !_exploded)
        {
            yield return new WaitForFixedUpdate();
            dist = (hit.point - (Vector2)rb.transform.position).magnitude;
            nextPoint = (Vector2)rb.transform.position + rb.linearVelocity * Time.fixedDeltaTime;
            nextDist = (hit.point - nextPoint).magnitude;
        }
        Explode(new ProjectileContactContext(hit.point, hit.collider.tag));
    }

    public override Vector2 SimulateProjectileBehaviorAndCalculateClosestPositionToTarget(Vector2 start, Vector2 target, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner)
    {
        float minDist = Vector2.Distance(start, target);
        Vector2 minPos = start;

        var numHits = Physics2D.RaycastNonAlloc(start, aimVector, raycastHitArray, Constants.ProjectileRaycastDistance, LayerMaskHelper.GetCombinedLayerMask(Constants.ProjectileCollisionLayers));
        var closestHit = raycastHitArray.Take(numHits).Where(hit => hit.collider != owner.Collider).OrderBy(hit => Vector2.Distance(hit.point, target)).FirstOrDefault();
        
        if (closestHit.collider == null)
        {
            return minPos;
        }
        else
        {
            if(Vector2.Distance(closestHit.point, target) <  minDist)
            {
                return closestHit.point;
            }
            else
            {
                return minPos;
            }
        }
    }
}