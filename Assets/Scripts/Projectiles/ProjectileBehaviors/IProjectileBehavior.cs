using System;
using System.Collections;
public interface IProjectileBehavior
{
    public event Action<ExplosionInfo> Exploded;
    public event Action<HitboxContactContext> ContactedWithoutExplosion;
    public void Launch(ProjectileLaunchContext context);
    public void OnContact(HitboxContactContext context);
    public void ForceExplode();
    public IEnumerator SimulateProjectileBehavior(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone);
}
