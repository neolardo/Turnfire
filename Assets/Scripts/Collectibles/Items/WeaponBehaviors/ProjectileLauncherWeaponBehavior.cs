using System;
using System.Collections;
using System.Linq;

public class ProjectileLauncherWeaponBehavior : UnityDriven, IItemBehavior
{
    public bool IsInUse => _isFiring;

    protected bool _isFiring;

    protected IProjectileBehavior _projectileBehavior;
    protected ProjectileLauncherWeaponDefinition _definition;

    public event Action ItemUsageFinished;

    public ProjectileLauncherWeaponBehavior(IProjectileBehavior projectileBehavior, ProjectileLauncherWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _projectileBehavior = projectileBehavior;
        _projectileBehavior.Exploded += OnProjectileExploded;
        _definition = definition;
    }

    public virtual void Use(ItemUsageContext context)
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
        ItemUsageFinished?.Invoke();
    }

    public virtual void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        rendererManager.SelectRenderer(ItemPreviewRendererType.Trajectory);
        rendererManager.TrajectoryRenderer.ToggleGravity(_definition.UseGravityForPreview);
        rendererManager.TrajectoryRenderer.SetOrigin(context.Owner.transform);
        rendererManager.TrajectoryRenderer.SetTrajectoryMultipler(_definition.FireStrength.CalculateValue());
    }
}
