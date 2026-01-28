using UnityEngine;

[CreateAssetMenu(fileName = "BulletDefinition", menuName = "Scriptable Objects/Projectiles/BulletDefinition")]
public class BulletProjectileDefinition : ProjectileDefinition
{
    public override IProjectileBehavior CreateProjectileBehavior()
    {
        return new BulletProjectileBehavior(this);
    }
}
