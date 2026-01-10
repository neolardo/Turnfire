public class GunWeaponBehavior : ProjectileLauncherWeaponBehavior
{
    public GunWeaponBehavior(IProjectileBehavior projectileBehavior, ProjectileLauncherWeaponDefinition definition) : base(projectileBehavior, definition)
    {
        IsAimingNormalized = true;
    }

    public override void Use(ItemUsageContext context)
    {
        var updatedContext = new ItemUsageContext(context.AimOrigin, context.AimVector.normalized, context.Owner, context.LaserRenderer);
        CreateAndLaunchProjectile(updatedContext);
    }
}
