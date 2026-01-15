using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ProjectileLauncherWeaponBehavior : WeaponBehavior
{
    protected IProjectileBehavior _projectileBehavior;
    protected ProjectileLauncherWeaponDefinition _definition;

    public ProjectileLauncherWeaponBehavior(IProjectileBehavior projectileBehavior, ProjectileLauncherWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _projectileBehavior = projectileBehavior;
        _projectileBehavior.Exploded += OnProjectileExploded;
        _definition = definition;
    }

    public override void Use(ItemUsageContext context)
    {
        StartCoroutine(CreateAndLaunchProjectile(context));
    }

    protected IEnumerator CreateAndLaunchProjectile(ItemUsageContext context)
    {
        _isAttacking = true;
        var p = GameServices.ProjectilePool.Get();
        yield return new WaitUntil(() => p.IsReady);
        p.Initialize(_definition.ProjectileDefinition, _projectileBehavior);
        p.Launch(context, _definition.FireStrength.CalculateValue());
    }

    private void OnProjectileExploded(ExplosionInfo ei)
    {
        StartCoroutine(WaitUntilExplosionFinished(ei));
    }

    private IEnumerator WaitUntilExplosionFinished(ExplosionInfo ei)
    {
        while (ei.ExplodedCharacters.Any(c => c.IsAlive && c.IsMoving) || ei.Explosion.IsExploding)
        {
            yield return null;
        }
        _isAttacking = false;
        InvokeItemUsageFinished();
    }

    public override void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        rendererManager.SelectRenderer(ItemPreviewRendererType.Trajectory);
        rendererManager.TrajectoryRenderer.ToggleGravity(_definition.UseGravityForPreview);
        rendererManager.TrajectoryRenderer.SetOrigin(context.Owner.ItemTransform);
        rendererManager.TrajectoryRenderer.SetTrajectoryMultipler(_definition.FireStrength.CalculateValue());
    }

    public override IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone)
    {
        var result = ItemBehaviorSimulationResult.None;
        // apply fire strength
        context = new ItemBehaviorSimulationContext(context, _definition.FireStrength.AvarageValue);
        yield return _projectileBehavior.SimulateProjectileBehavior(context, (r) => result = r);
        onDone?.Invoke(result);
    }
}
