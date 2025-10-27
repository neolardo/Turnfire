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
        rb.linearVelocity = Vector2.zero;
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

    protected override void Explode(ProjectileContactContext context)
    {
        if(_exploded)
        {
            return;
        }
        base.Explode(context);
        _exploded = true;
    }

}
