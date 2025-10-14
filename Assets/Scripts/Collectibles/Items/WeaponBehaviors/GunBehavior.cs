using System.Collections;
using System.Linq;

public class GunBehavior : UnityDriven, IItemBehavior
{
    public bool IsInUse => _isFiring;

    private bool _isFiring;
    private IProjectileBehavior _projectileBehavior;
    private GunDefinition _definition;

    public GunBehavior(IProjectileBehavior projectileBehavior, GunDefinition definition) : base(CoroutineRunner.Instance)
    {
        _projectileBehavior = projectileBehavior;
        _projectileBehavior.Exploded += OnProjectileExploded;
        _definition = definition;
    }

    public void Use(ItemUsageContext context)
    {
        _isFiring = true;
        var p = context.ProjectileManager.GetProjectile();
        p.Initialize(_definition.ProjectileDefinition, _projectileBehavior);
        p.Launch(context, _definition.FireStrength.CalculateValue());
    }

    private void OnProjectileExploded(ExplosionInfo ei)
    {
        StartCoroutine(WaitUntilFiringFinished(ei));
    }

    private IEnumerator WaitUntilFiringFinished(ExplosionInfo ei)
    {
        while (ei.ExplodedCharacters.Any(c => c.IsAlive && c.IsMoving) || ei.Explosion.IsAnimationPlaying)
        {
            yield return null;
        }
        _isFiring = false;
    }

    public void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        rendererManager.SelectRenderer(ItemPreviewRendererType.Trajectory);
        rendererManager.TrajectoryRenderer.SetOrigin(context.Owner.transform);
        rendererManager.TrajectoryRenderer.SetTrajectoryMultipler(_definition.FireStrength.CalculateValue());
    }

}
