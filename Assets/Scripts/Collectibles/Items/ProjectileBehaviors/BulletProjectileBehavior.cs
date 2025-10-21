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
        var rb = context.ProjectileRigidbody;
        float angle = Mathf.Atan2(context.AimVector.y, context.AimVector.x) * Mathf.Rad2Deg;
        rb.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rb.transform.position = context.AimOrigin + context.AimVector.normalized * Constants.ProjectileOffset;
        rb.gravityScale = 0;
        rb.linearVelocity = context.AimVector / rb.mass;
        TryContactImmadiatelyOnLaunchIfNearAnyCollider(context);

    }


}