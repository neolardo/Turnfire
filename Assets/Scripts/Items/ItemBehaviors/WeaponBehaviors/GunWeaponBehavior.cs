public class GunWeaponBehavior : ProjectileLauncherWeaponBehavior
{
    public GunWeaponBehavior(IProjectileBehavior projectileBehavior, ProjectileLauncherWeaponDefinition definition) : base(projectileBehavior, definition)
    {

    }

    public override void Use(ItemUsageContext context)
    {
        _isFiring = true;
        var p = context.ProjectilePool.Get();
        p.Initialize(_definition.ProjectileDefinition, _projectileBehavior);
        // aimvector is normalized since guns should not have variable strength
        var updatedContext = new ItemUsageContext(context.AimOrigin, context.AimVector.normalized, context.Owner, context.LaserRenderer, context.ProjectilePool);
        p.Launch(updatedContext, _definition.FireStrength.CalculateValue());
    }
}
