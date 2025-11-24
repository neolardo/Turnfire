using System;
using System.Collections;
using System.Collections.Generic;
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
        _isFiring = true;
        var p = context.ProjectilePool.Get();
        p.Initialize(_definition.ProjectileDefinition, _projectileBehavior);
        p.Launch(context, _definition.FireStrength.CalculateValue());
    }

    private void OnProjectileExploded(ExplosionInfo ei)
    {
        StartCoroutine(WaitUntilFiringFinished(ei));
    }

    private IEnumerator WaitUntilFiringFinished(ExplosionInfo ei)
    {
        while (ei.ExplodedCharacters.Any(c => c.IsAlive && c.IsMoving) || ei.Explosion.IsExploding)
        {
            yield return null;
        }
        _isFiring = false;
        InvokeItemUsageFinished();
    }

    public override void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        rendererManager.SelectRenderer(ItemPreviewRendererType.Trajectory);
        rendererManager.TrajectoryRenderer.ToggleGravity(_definition.UseGravityForPreview);
        rendererManager.TrajectoryRenderer.SetOrigin(context.Owner.transform);
        rendererManager.TrajectoryRenderer.SetTrajectoryMultipler(_definition.FireStrength.CalculateValue());
    }

    public override Vector2 SimulateWeaponBehaviorAndCalculateDestination(Vector2 start, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner, IEnumerable<Character> others, bool asd)
    {
        return _projectileBehavior.SimulateProjectileBehaviorAndCalculateDestination(start, aimVector * _definition.FireStrength.CalculateValue(), terrain, owner, others,  asd);
    }
}
