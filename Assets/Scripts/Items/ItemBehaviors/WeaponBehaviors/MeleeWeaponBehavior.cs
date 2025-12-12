using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MeleeWeaponBehavior : WeaponBehavior
{
    private MeleeWeaponDefinition _definition;

    private const float WeaponUsageDuration = 1f;
    private bool _isSimulated;
    private List<Character> _contactedCharactersSimulationResult;

    // bot evaluation
    private Character _lastOwner;
    private bool _anyContacts;
    public MeleeWeaponBehavior(MeleeWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        IsAimingNormalized = true;
        _contactedCharactersSimulationResult = new List<Character>();
    }

    public override void Use(ItemUsageContext context)
    {
        _lastOwner = context.Owner;
        _anyContacts = false;

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
        if(!_anyContacts)
        {
            BotEvaluationStatistics.GetData(_lastOwner.Team).TotalNonDamagingAttackCount++;
        }
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
            if(_isSimulated)
            {
                _contactedCharactersSimulationResult.Add(c);
            }
            else
            {
                var damage = _definition.Damage.CalculateValue();
                var data = BotEvaluationStatistics.GetData(_lastOwner.Team);
                if (c.Team == _lastOwner.Team)
                {
                    data.TotalDamageDealtToAllies += damage;
                }
                else
                {
                    data.TotalDamageDealtToEnemies += damage;
                }
                _anyContacts = true;

                c.Damage(damage);
            }
        }
    }

    public override void InitializePreview(ItemUsageContext context, ItemPreviewRendererManager rendererManager)
    {
        rendererManager.SelectRenderer(ItemPreviewRendererType.Trajectory);
        rendererManager.TrajectoryRenderer.ToggleGravity(false);
        rendererManager.TrajectoryRenderer.SetOrigin(context.Owner.ItemTransform);
        rendererManager.TrajectoryRenderer.SetTrajectoryMultipler(1);
    }

    public override IEnumerator SimulateUsage(ItemBehaviorSimulationContext context, Action<ItemBehaviorSimulationResult> onDone)
    {
        var hitbox = context.Owner.MeleeHitbox;
        _isSimulated = true;
        _contactedCharactersSimulationResult.Clear();

        var originalHitboxPosition = hitbox.transform.position;
        hitbox.transform.position = context.Origin;
        hitbox.Initialize(_definition.AttackSectorAngleDegrees.CalculateValue(), _definition.AttackRange.CalculateValue());
        hitbox.Rotate(context.AimVector);
        hitbox.Contacted += OnWeaponHitboxContacted;
        hitbox.gameObject.SetActive(true);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        float damage = _definition.Damage.AvarageValue;
        float allyDamage = 0;
        float enemyDamage = 0;

        foreach (var c in _contactedCharactersSimulationResult)
        {
            if (c.Team == context.Owner.Team)
            {
                allyDamage += damage;
            }
            else
            {
                enemyDamage += damage;
            }
        }

        hitbox.gameObject.SetActive(false);
        hitbox.Contacted -= OnWeaponHitboxContacted;
        hitbox.transform.position = originalHitboxPosition;
        _isSimulated = false;

        onDone?.Invoke(ItemBehaviorSimulationResult.Damage(context.Origin, enemyDamage, allyDamage));
    }
}
