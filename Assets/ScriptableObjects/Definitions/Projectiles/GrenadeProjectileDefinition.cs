using UnityEngine;

[CreateAssetMenu(fileName = "GrenadeProjectileDefinition", menuName = "Scriptable Objects/Projectiles/GrenadeProjectileDefinition")]
public class GrenadeProjectileDefinition : ProjectileDefinition
{
    public int ExplosionContactThreshold;
    public int ExplosionDelaySeconds;
    public PhysicsMaterial2D GrenadePhysicsMaterial;

    public override IProjectileBehavior CreateProjectileBehavior()
    {
        return new GrenadeProjectileBehavior(this);
    }
}
