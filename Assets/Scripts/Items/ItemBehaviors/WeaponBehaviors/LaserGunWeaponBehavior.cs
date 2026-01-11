using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserGunWeaponBehavior : WeaponBehavior
{
    private LaserGunWeaponDefinition _definition;
    private RaycastHit2D[] _raycastHitArray;
    private const float _visualStartDirectionalOffset = .01f;
    private readonly Vector2 _visualStartGlobalOffset = new Vector2(0f, 0.05f);
    private const float _requiredSafeRadius = .2f;

    public LaserGunWeaponBehavior(LaserGunWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        IsAimingNormalized = true;
        _raycastHitArray = new RaycastHit2D[Constants.RaycastHitColliderNumMax];
    }

    public override void Use(ItemUsageContext context)
    {
        _isAttacking = true;
        StartCoroutine(StartLaserAndDamageCharactersOnContact(context);
    }

    public IEnumerator StartLaserAndDamageCharactersOnContact(ItemUsageContext context)
    {
        var laser = GameServices.LaserPool.Get();
        var owner = context.Owner;
        yield return new WaitUntil(() => laser.IsReady);
        laser.Initialize(_definition.MaximumBounceCount.CalculateValue(), _definition.MaximumDistance.CalculateValue());
        laser.StartLaser(context.AimOrigin, context.AimVector.normalized);
        var hitCharacters = laser.GetHitCharacters().ToList();
        var removableCharacters = new List<Character>();
        while(laser.IsAnimationInProgress)
        {
            foreach (var c in hitCharacters)
            {
                if (c == owner && laser.IsFirstRayRendered)
                {
                    continue;
                }

                if (c.OverlapPoint(laser.LaserHead.position))
                { 
                    c.TakeDamage(_definition, _definition.Damage.CalculateValue());
                    removableCharacters.Add(c);
                }
            }
            foreach(var c in removableCharacters)
            {
                hitCharacters.Remove(c);
            }
            removableCharacters.Clear();

            yield return new WaitForFixedUpdate();
        }
        // if any unhit characters remain beause of the yield interval, damage now
        foreach (var c in hitCharacters)
        {
            c.TakeDamage(_definition, _definition.Damage.CalculateValue());
        }
        _isAttacking = false;
        InvokeItemUsageFinished();
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
        int damage = _definition.Damage.AvarageValue;
        int damageToAllies = 0;
        int damageToEnemies = 0;
        CalculateLaserPath(context.Owner, context.Origin, context.AimVector, out var hitCharacters);
        Vector2 closestDamagingPosition = hitCharacters.Count == 0 ? context.Origin : hitCharacters.First().transform.position;
        float minDist = Vector2.Distance(closestDamagingPosition, context.Origin);
        foreach (var c in hitCharacters)
        {
            if (c.Team == context.Owner.Team)
            {
                damageToAllies += damage;
            }
            else
            {
                damageToEnemies += damage;
            }
            var dist = Vector2.Distance(context.Origin, c.transform.position);
            if (dist < minDist)
            {
                closestDamagingPosition = c.transform.position;
                minDist = dist;
            }
        }
        onDone?.Invoke(ItemBehaviorSimulationResult.Damage(closestDamagingPosition, damageToEnemies, damageToAllies));
        yield return null;
    }

}
