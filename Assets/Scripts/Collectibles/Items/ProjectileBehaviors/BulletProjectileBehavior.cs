using System;
using UnityEngine;

public class BulletProjectileBehavior : UnityDriven, IProjectileBehavior
{

    public event Action<ExplosionInfo> Exploded;

    BulletProjectileDefinition _definition;
    public BulletProjectileBehavior(BulletProjectileDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
    }

    public void Launch(ProjectileLaunchContext context) 
    {
        context.ProjectileRigidbody.linearVelocity = Vector2.zero;
        context.ProjectileRigidbody.transform.position = context.AimOrigin + context.AimVector.normalized * Constants.ProjectileOffset;
        context.ProjectileRigidbody.AddForce(context.AimVector, ForceMode2D.Impulse);
    }

    public void OnContact(ProjectileContactContext context)
    {
        var damage = _definition.Damage.CalculateValue();
        var exp = context.ExplosionPool.Get();
        exp.Initialize(_definition.ExplosionDefinition);
        var explodedCharacters = exp.Explode(context.ContactPoint, damage);
        Exploded?.Invoke(new ExplosionInfo(explodedCharacters, context.Projectile, exp)); //TODO: refactor
    }


}
