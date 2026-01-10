using UnityEngine;

[CreateAssetMenu(fileName = "CannonBallDefinition", menuName = "Scriptable Objects/Projectiles/CannonBallDefinition")]
public class CannonBallProjectileDefinition : ProjectileDefinition
{
    public override IProjectileBehavior CreateProjectileBehavior()
    {
        return new CannonBallProjectileBehavior(this);
    }
}
