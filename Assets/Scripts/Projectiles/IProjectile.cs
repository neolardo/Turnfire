using System;

public interface IProjectile
{
    bool IsReady { get; } 

    event Action<IProjectile> Exploded;
    void Initialize(ProjectileDefinition definition, IProjectileBehavior behavior);
    void Launch(ItemUsageContext itemContext, float fireStrength);
    void ForceExplode();
}
