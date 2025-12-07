using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehavior : WeaponBehavior
{
    private MeleeWeaponDefinition _definition;

    private const float WeaponUsageDuration = 1f; //TODO

    public MeleeWeaponBehavior(MeleeWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
    }

    public override void Use(ItemUsageContext context)
    {
        _isAttacking = true;
        var hitbox = context.Owner.MeleeHitbox;
        InitializeHitbox(hitbox, context.AimVector);
        StartCoroutine(WaitUntilWeaponUsageFinished(hitbox));
    }

    private void InitializeHitbox(SectorHitbox hitbox, Vector2 aimVector)
    {
        hitbox.Initialize(_definition.AttackSectorAngleDegrees.CalculateValue(), _definition.AttackRange.CalculateValue());
        hitbox.Rotate(aimVector);
        hitbox.Contacted += OnWeaponHitboxContacted;
        hitbox.gameObject.SetActive(true);
    }

    private IEnumerator WaitUntilWeaponUsageFinished(SectorHitbox hitbox)
    {
        yield return new WaitForSeconds(WeaponUsageDuration);
        _isAttacking = false;
        hitbox.gameObject.SetActive(false);
        hitbox.Contacted -= OnWeaponHitboxContacted;
        InvokeItemUsageFinished();
    }

    public void OnWeaponHitboxContacted(HitboxContactContext context)
    {
        var c = context.Collider.GetComponent<Character>();
        if(c != null)
        {
            c.Damage(_definition.Damage.CalculateValue());
        }
    }

    public override void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        rendererManager.SelectRenderer(ItemPreviewRendererType.Trajectory);
        rendererManager.TrajectoryRenderer.ToggleGravity(false);
        rendererManager.TrajectoryRenderer.SetOrigin(context.Owner.ItemTransform);
        rendererManager.TrajectoryRenderer.SetTrajectoryMultipler(1);
    }

    public override WeaponBehaviorSimulationResult SimulateWeaponBehavior(Vector2 start, Vector2 aimVector, DestructibleTerrainManager terrain, Character owner, IEnumerable<Character> others)
    {
        //TODO
        return WeaponBehaviorSimulationResult.Zero;
    }
}
