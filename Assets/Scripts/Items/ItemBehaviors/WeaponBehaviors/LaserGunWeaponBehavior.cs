using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserGunWeaponBehavior : WeaponBehavior
{
    private LaserGunWeaponDefinition _definition;

    private LaserPhysics _simulationLaserPhysics;
  

    public LaserGunWeaponBehavior(LaserGunWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        IsAimingNormalized = true;
        _simulationLaserPhysics = new LaserPhysics();
    }

    public override void Use(ItemUsageContext context)
    {
        _isAttacking = true;
        StartCoroutine(StartLaserAndDamageCharactersOnContact(context));
    }

    public IEnumerator StartLaserAndDamageCharactersOnContact(ItemUsageContext context)
    {
        var laser = GameServices.LaserPool.Get();
        var owner = context.Owner;
        yield return new WaitUntil(() => laser.IsReady);
        laser.Initialize(_definition.MaximumBounceCount.CalculateValue(), _definition.MaximumDistance.CalculateValue());
        laser.StartBeam(context.AimOrigin, context.AimVector.normalized, owner);
        var hitCharacters = laser.GetHitCharacters().ToList();
        var removableCharacters = new List<Character>();
        while(laser.IsBeamAnimationInProgress)
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
        _simulationLaserPhysics.Initialize(_definition.MaximumBounceCount.AvarageValue, _definition.MaximumDistance.AvarageValue);
        _simulationLaserPhysics.CalculateLaserPath(context.Origin, context.AimVector, context.Owner);
        var hitCharacters = _simulationLaserPhysics.GetHitCharacters().ToList();
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
