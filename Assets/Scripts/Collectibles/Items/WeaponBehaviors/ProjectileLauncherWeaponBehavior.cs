using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileLauncherWeaponBehavior : WeaponBehavior
{
    protected IProjectileBehavior _projectileBehavior;
    protected ProjectileLauncherWeaponDefinition _definition;

    //stats
    protected Character _lastOwner;
    public ProjectileLauncherWeaponBehavior(IProjectileBehavior projectileBehavior, ProjectileLauncherWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _projectileBehavior = projectileBehavior;
        _projectileBehavior.Exploded += OnProjectileExploded;
        _definition = definition;
    }

    public override void Use(ItemUsageContext context)
    {
        _lastOwner = context.OwnerCollider.GetComponent<Character>();
        _isFiring = true;
        var p = context.ProjectilePool.Get();
        p.Initialize(_definition.ProjectileDefinition, _projectileBehavior);
        p.Launch(context, _definition.FireStrength.CalculateValue());
    }

    private void OnProjectileExploded(ExplosionInfo ei)
    {
        if (!ei.ExplodedCharacters.Any())
        {
            BotEvaluationStatistics.GetData(_lastOwner.Team).TotalNonDamagingProjectileCount++;
        }
        else
        {
            foreach (var character in ei.ExplodedCharacters)
            {
                var data = BotEvaluationStatistics.GetData(character.Team);
                if(character.Team == _lastOwner.Team)
                {
                    data.TotalDamageDealtToAllies += ei.Damage;
                }
                else
                {
                    data.TotalDamageDealtToEnemies += ei.Damage;
                }
                if(character == _lastOwner && !_lastOwner.IsAlive)
                {
                    data.TotalSuicideCount++;
                }
            }
        }
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

    public override WeaponBehaviorSimulationResult SimulateWeaponBehavior(Vector2 start, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner, IEnumerable<Character> others)
    {
        return _projectileBehavior.SimulateProjectileBehavior(start, aimVector * _definition.FireStrength.CalculateValue(), terrain, owner, others);
    }
}
