using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserGunWeaponBehavior : WeaponBehavior
{
    private LaserGunWeaponDefinition _definition;
    private RaycastHit2D[] _raycastHitArray;
    private const float _visualStartOffset = .2f;
    private const float _requiredSafeRadius = .3f;

    public LaserGunWeaponBehavior(LaserGunWeaponDefinition definition) : base(CoroutineRunner.Instance)
    {
        _definition = definition;
        IsAimingNormalized = true;
        _raycastHitArray = new RaycastHit2D[Constants.RaycastHitColliderNumMax];
        FastSimAvailable = true;
    }

    public override void Use(ItemUsageContext context)
    {
        _isAttacking = true;
        var points = CalculateLaserPath(context.Owner, context.AimOrigin, context.AimVector, out var hitCharacters);
        AddBotStats(context.Owner, hitCharacters);
        context.LaserRenderer.StartLaser(points.ToArray());
        StartCoroutine(FollowLaserAndDamageCharactersOnContact(context.Owner, hitCharacters, context.LaserRenderer));
    }

    private void AddBotStats(Character owner, IEnumerable<Character> hitCharacters)
    {
        var damage = _definition.Damage.CalculateValue();
        var data = BotEvaluationStatistics.GetData(owner.Team);
        float allyDamage = 0;
        float enemyDamage = 0;
        foreach (var c in hitCharacters)
        {
            if (c.Team == owner.Team)
            {
                allyDamage += damage;
            }
            else
            {
                enemyDamage += damage;
            }
        }
        data.DamageDealtToAllies += allyDamage;
        data.DamageDealtToEnemies += enemyDamage;
        if (!hitCharacters.Any())
        {
            data.NonDamagingAttackCount++;
        }
    }

    private IEnumerable<Vector2> CalculateLaserPath(Character owner, Vector2 origin, Vector2 direction, out HashSet<Character> hitCharacters)
    {
        int maxBounces = _definition.MaximumBounceCount.CalculateValue();
        float maxDistance = _definition.MaximumDistance.CalculateValue();
        
        var points = new List<Vector2>();
        hitCharacters = new HashSet<Character>();

        origin += direction.normalized * _visualStartOffset;
        if (SafeObjectPlacer.TryFindSafePosition(origin, direction, LayerMaskHelper.GetLayerMask(Constants.GroundLayer), _requiredSafeRadius, out var safePosition))
        {
            origin = safePosition;
        }

        points.Add(origin);

        Vector2 currentPos = origin;
        Vector2 currentDir = direction.normalized;
        var mask = LayerMaskHelper.GetCombinedLayerMask(Constants.GroundLayer, Constants.CharacterLayer);
        var filter = new ContactFilter2D();
        filter.SetLayerMask(mask);

        for (int i = 0; i < maxBounces; i++)
        {
            int numHits = Physics2D.Raycast(currentPos, currentDir, filter,_raycastHitArray, maxDistance);
            var hits = _raycastHitArray.Take(numHits);
            var closestGroundHit = hits.Where(hit => hit.collider != null && hit.collider.tag == Constants.GroundTag).OrderBy(hit => hit.distance).FirstOrDefault();
            var characterHits = hits.Where(hit => hit.collider != null && hit.collider.tag == Constants.CharacterTag);

            foreach(var cHit in characterHits)
            {
                cHit.collider.TryGetComponent<Character>(out var c);

                if (i == 0 && c == owner)
                {
                    continue;
                }

                hitCharacters.Add(c);
            }

            if (closestGroundHit.collider == null)
            {
                points.Add(currentPos + currentDir * maxDistance);
                break;
            }

            var hit = closestGroundHit;

            points.Add(hit.point);

            currentDir = Vector2.Reflect(currentDir, hit.normal);

            // Move slightly away to avoid self-hitting
            currentPos = hit.point + currentDir * 0.01f;
        }

        return points;
    }

    public IEnumerator FollowLaserAndDamageCharactersOnContact(Character owner, HashSet<Character> hitCharacters, PixelLaserRenderer laserRenderer)
    {
        var removableCharacters = new List<Character>();
        while(laserRenderer.IsAnimationInProgress)
        {
            foreach (var c in hitCharacters)
            {
                if (c == owner && laserRenderer.IsFirstRay)
                {
                    continue;
                }

                if (c.OverlapPoint(laserRenderer.LaserHead.position))
                { 
                    c.Damage(_definition.Damage.CalculateValue());
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
            c.Damage(_definition.Damage.CalculateValue());
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

    public override ItemBehaviorSimulationResult SimulateUsageFast(ItemBehaviorSimulationContext context)
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
        return ItemBehaviorSimulationResult.Damage(closestDamagingPosition, damageToEnemies, damageToAllies);
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
