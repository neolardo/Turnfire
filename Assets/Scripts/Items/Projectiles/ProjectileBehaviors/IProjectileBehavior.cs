using System;
using System.Collections;
public interface IProjectileBehavior
{
    public event Action<ExplosionInfo> Exploded;
    public void Launch(ProjectileLaunchContext context);
    public void OnContact(HitboxContactContext context);
    public void SetProjectile(Projectile projectile);
    public void ForceExplode();
    public IEnumerator SimulateProjectileBehavior(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone);
    public ItemBehaviorSimulationResult SimulateProjectileBehaviorFast(ItemBehaviorSimulationContext context);
}
