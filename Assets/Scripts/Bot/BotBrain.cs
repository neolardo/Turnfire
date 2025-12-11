using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BotBrain : UnityDriven
{
    private BotTuningDefinition _tuning;

    public event Action<BotGoal> GoalDecided;

    // utility constants
    private const float DamageUtilitySoftBoxTemperature = 0.06f;
    private const float PackageJumpDistanceUtilityExponentBase = 0.8f;
    private const float DamageCenterDistanceUtilityExponentBase = 0.55f;
    private const float OffensiveEnemyDistanceUtilityRiseDecayMidpoint = 0.8f;
    private const float OffensiveEnemyDistanceUtilityRiseSteepness = -4f;
    private const float OffensiveEnemyDistanceUtilityRiseFallMidpoint = 2f;
    private const float OffensiveEnemyDistanceUtilityFallDecayMidpoint = 5f;
    private const float OffensiveEnemyDistanceUtilityFallSteepness = 1.5f;
    private const float DefensiveEnemyDistanceUtilityDecayMidpoint = 5f;
    private const float DefensiveEnemyDistanceUtilitySteepness = -1f;
    private const float MobilityUtilityDecayMidpoint = 1.5f;
    private const float MobilityUtilitySteepness = -2f;
    private const float HealingUtilityDecayMidpoint = 2f;
    private const float HealingUtilitySteepness = -0.7f;
    private const float ArmorUtilityDecayMidpoint = 1.7f;
    private const float ArmorUtilitySteepness = -1.1f;
    private const float TravelDistanceUtilityDecayMidpointAtMinSensitivity = 10;
    private const float TravelDistanceUtilityDecayMidpointAtMaxSensitivity = 2;
    private const float TravelDistanceUtilitySteepnessAtMinSensitivity = 0.5f;
    private const float TravelDistanceUtilitySteepnessAtMaxSensitivity = 2;
    private float _travelDistanceUtilityDecayMidpoint;
    private float _travelDistanceUtilitySteepness;

    public BotBrain(BotTuningDefinition tuning, MonoBehaviour coroutineManager) : base(coroutineManager)
    {
        _tuning = tuning;
        CacheUtilityParameters();
    }
    
    public void BeginThinking(BotContext context)
    {
        StartCoroutine(DecideGoal(context));
    }
    
    private IEnumerator DecideGoal(BotContext context)
    {
        BotGoal goal = default;
        if (context.ActionState == CharacterActionStateType.ReadyToMove)
        {
            yield return DecideGoalWhenReadyToMove(context, g => goal = g);
        }
        else if (context.ActionState == CharacterActionStateType.ReadyToUseItem)
        {
            yield return DecideGoalWhenReadyToUseItem(context, g => goal = g);
        }
        GoalDecided?.Invoke(goal);
    }

    #region Moving

    private IEnumerator DecideGoalWhenReadyToMove(BotContext context, Action<BotGoal> onDone)
    {
        var startPoint = context.JumpGraph.FindClosestStandingPoint(context.Self.FeetPosition);
        var possiblePositions = context.JumpGraph.GetAllReachableStandingPointsFromPoint(startPoint);

        float bestScore = float.NegativeInfinity;
        StandingPoint bestPoint = default;

        foreach (var targetPoint in possiblePositions)
        {
            float score = EvaluatePositionScore(startPoint, targetPoint, context);
            if (score > bestScore)
            {
                bestScore = score;
                bestPoint = targetPoint;
            }
            yield return null;
        }

        if (bestPoint == startPoint)
        {
            onDone?.Invoke(BotGoal.SkipAction());
        }
        else
        {
            onDone?.Invoke(BotGoal.Move(bestPoint));
        }
    }

    private float EvaluatePositionScore(StandingPoint startPoint, StandingPoint targetPoint, BotContext context)
    {
        float score = 0;

        // offense
        var closestEnemyDistance = context.Enemies.Select(e => Vector2.Distance(targetPoint.WorldPos, e.transform.position)).DefaultIfEmpty(float.PositiveInfinity).Min();
        score += OffensiveEnemyDistanceUtility(closestEnemyDistance) * _tuning.Offense;

        // defense
        if (context.Self.NormalizedHealth < _tuning.LowHealthThreshold)
        {
            var avarageEnemyDistance = context.Enemies.Select(e => Vector2.Distance(targetPoint.WorldPos, e.transform.position)).DefaultIfEmpty(float.PositiveInfinity).Average();
            score += DefensiveEnemyDistanceUtility(avarageEnemyDistance) * _tuning.Defense;
        }

        // package
        float bestPackageScore = 0;
        var weapons = context.Self.GetAllItems().Where(i => i.Definition.ItemType == ItemType.Weapon);
        int remainingTotalAmmo = weapons.Sum(i => i.Quantity);
        float packageGreed = _tuning.GeneralPackageGreed;
        if (remainingTotalAmmo == 0)
        {
            packageGreed = _tuning.OutOfAmmoPackageGreed;
        }
        else if (remainingTotalAmmo <= _tuning.RemainingAmmoLowThreshold)
        {
            packageGreed = _tuning.RemainingAmmoLowPackageGreed;
        }

        foreach (Package p in context.Packages)
        {
            var packagePos = context.JumpGraph.FindClosestStandingPoint(p.transform.position);
            if (context.JumpGraph.TryCalculateJumpDistanceBetween(targetPoint, packagePos, out var numJumps))
            {
                float packageScore = PackageJumpDistanceUtility(numJumps);
                if (packageScore > bestPackageScore)
                {
                    bestPackageScore = packageScore;
                }
            }
        }
        score += bestPackageScore * packageGreed;

        //travel distance
        if (context.JumpGraph.TryCalculateJumpDistanceBetween(startPoint, targetPoint, out int travelDistance))
        {
            score += TravelDistanceUtility(travelDistance);
        }

        // decision randomness
        score += GetDecisionRandomnessBias();
        return score;
    }

    private float GetDecisionRandomnessBias()
    {
        return UnityEngine.Random.Range(-_tuning.DecisionRandomnessBias / 2f, _tuning.DecisionRandomnessBias / 2f);
    }


    #endregion

    #region Shooting

    private IEnumerator DecideGoalWhenReadyToUseItem(BotContext context, Action<BotGoal> onDone)
    {
        var startPoint = context.JumpGraph.FindClosestStandingPoint(context.Self.FeetPosition);
        var possiblePositions = context.JumpGraph.GetAllReachableStandingPointsFromPoint(startPoint);

        float bestScore = float.NegativeInfinity;
        BotGoal bestGoal = BotGoal.SkipAction();

        var items = context.Self.GetAllItems();

        foreach (var item in items)
        {
            float score = float.NegativeInfinity;
            BotGoal goal = BotGoal.SkipAction();
            yield return EvaluateItemScore(item, context, (s, g) => { score = s; goal = g; });
            if (score > bestScore)
            {
                bestScore = score;
                bestGoal = goal;
            }
            yield return null;
        }

        onDone?.Invoke(bestGoal);       
    }

    private IEnumerator EvaluateItemScore(Item item, BotContext context, Action<float, BotGoal> onDone)
    {
        var startPos = context.Self.ItemTransform.position;
        var simulationContext = new ItemBehaviorSimulationContext(context.Self, context.TeamMates.Concat(context.Enemies), startPos, Vector2.zero, context.Terrain);
        float score = float.NegativeInfinity;
        if (item.Definition.ItemType == ItemType.Weapon)
        {
            var weaponBehavior = item.Behavior as WeaponBehavior;
            float pureDamageScore = float.NegativeInfinity;
            Vector2 attackVector = default;
            yield return CalculateBestAttackVector(startPos, weaponBehavior, context, (vec, s, pds) => { score = s; attackVector = vec; pureDamageScore = pds; });
            bool remainingAmmoLow = !item.Definition.IsQuantityInfinite && item.Quantity <= _tuning.RemainingAmmoLowThreshold && item.Definition.MaximumQuantity > _tuning.RemainingAmmoLowThreshold;
            if (!remainingAmmoLow || pureDamageScore > 0) // if low on ammo only attack if can deal damage
            {
                attackVector = ApplyAimRandomness(attackVector);
                onDone?.Invoke(score, BotGoal.Attack(attackVector, item));
            }
            else
            {
                onDone?.Invoke(0, BotGoal.SkipAction());
            }
        }
        else
        {
            var result = ItemBehaviorSimulationResult.None;
            yield return item.Behavior.SimulateUsage(simulationContext, (r) => result = r);
            score += HealingUtility(result.TotalHealingDone) * _tuning.Defense;
            score += ArmorUtility(result.TotalArmorBoost) * _tuning.Defense;
            score += MobilityUtility(result.TotalMobilityBoost) * _tuning.MobilityPreference;
            onDone?.Invoke(score, BotGoal.UseItem(item));
        }
    }

    public IEnumerator CalculateBestAttackVector(Vector2 start, WeaponBehavior weaponBehavior, BotContext context, Action<Vector2, float, float> onDone)
    {
        float bestScore = float.NegativeInfinity;
        float bestPureDamageScore = float.NegativeInfinity;
        Vector2 bestShot = default;
        var others = context.TeamMates.Concat(context.Enemies);

        for (float angle = -30; angle < 210f; angle += Constants.AimAngleSimulationStep) // only check valid shoot angles
        {
            Vector2 direction = angle.AngleDegreesToVector();
            for (float strength = Constants.AimStrengthSimulationStep; strength <= 1f; strength += Constants.AimStrengthSimulationStep)
            {
                Vector2 aimVector = direction * strength;

                var simulationContext = new ItemBehaviorSimulationContext(context.Self, others, start, aimVector, context.Terrain);
                var simulationResult = ItemBehaviorSimulationResult.None;

                yield return weaponBehavior.SimulateUsage(simulationContext, (result) => simulationResult = result);

                var minDistFromClosestEnemy = context.Enemies.Select(c => Vector2.Distance(c.transform.position, simulationResult.DamageCenter)).DefaultIfEmpty(float.PositiveInfinity).Min();

                float pureDamageScore = DamageUtility(simulationResult.TotalDamageDealtToEnemies) * _tuning.Offense;
                pureDamageScore += -DamageUtility(simulationResult.TotalDamageDealtToAllies) * _tuning.Defense;
                float distanceScore = DamageCenterDistanceUtility(minDistFromClosestEnemy) * _tuning.Offense;

                if (pureDamageScore + distanceScore > bestScore)
                {
                    bestScore = pureDamageScore + distanceScore;
                    bestPureDamageScore = pureDamageScore;
                    bestShot = aimVector;
                }

                if(weaponBehavior.IsAimingNormalized)
                {
                    break;
                }
            }
            yield return null;
        }

        onDone?.Invoke(bestShot, bestScore, bestPureDamageScore);
    }

    private Vector2 ApplyAimRandomness(Vector2 aimVector)
    {
        var angle = aimVector.ToAngleDegrees();
        angle += UnityEngine.Random.Range(-_tuning.AimRandomnessBias / 2f, _tuning.AimRandomnessBias / 2f) * 360f;
        var newDirection = angle.AngleDegreesToVector();
        var newMagnitude = Mathf.Clamp01(aimVector.magnitude + UnityEngine.Random.Range(-_tuning.AimRandomnessBias / 2f, _tuning.AimRandomnessBias / 2f));
        return newDirection * newMagnitude;
    }

    #endregion

    #region Normalizing Utility Functions

    private float DamageUtility(float damage)
    {
        return 1f - Mathf.Exp(-damage * DamageUtilitySoftBoxTemperature);
    }

    private float OffensiveEnemyDistanceUtility(float closestEnemyDistance)
    {
        if(closestEnemyDistance < OffensiveEnemyDistanceUtilityRiseFallMidpoint)
        {
            return MathHelpers.Sigmoid(closestEnemyDistance, OffensiveEnemyDistanceUtilityRiseDecayMidpoint, OffensiveEnemyDistanceUtilityRiseSteepness);
        }
        else 
        {
            return MathHelpers.Sigmoid(closestEnemyDistance, OffensiveEnemyDistanceUtilityFallDecayMidpoint, OffensiveEnemyDistanceUtilityFallSteepness);
        }
    }

    private float DefensiveEnemyDistanceUtility(float avarageEnemyDistance)
    {
        return MathHelpers.Sigmoid(avarageEnemyDistance, DefensiveEnemyDistanceUtilityDecayMidpoint, DefensiveEnemyDistanceUtilitySteepness);
    }

    private float MobilityUtility(float mobilityBoost)
    {
        return MathHelpers.Sigmoid(mobilityBoost, MobilityUtilityDecayMidpoint, MobilityUtilitySteepness);
    }

    private float HealingUtility(float healing)
    {
        return MathHelpers.Sigmoid(healing, HealingUtilityDecayMidpoint, HealingUtilitySteepness);
    }
    private float ArmorUtility(int armorDurability)
    {
        return MathHelpers.Sigmoid(armorDurability, ArmorUtilityDecayMidpoint, ArmorUtilitySteepness);
    }

    private float DamageCenterDistanceUtility(float damageCenterDistance)
    {
        return Mathf.Pow(DamageCenterDistanceUtilityExponentBase, damageCenterDistance);
    }

    private float PackageJumpDistanceUtility(int numJumps)
    {
        return Mathf.Pow(PackageJumpDistanceUtilityExponentBase, numJumps);
    }
    private float TravelDistanceUtility(int numJumps)
    {
        return MathHelpers.Sigmoid(numJumps, _travelDistanceUtilityDecayMidpoint, _travelDistanceUtilitySteepness);
    }

    private void CacheUtilityParameters()
    {
        _travelDistanceUtilityDecayMidpoint = Mathf.Lerp(TravelDistanceUtilityDecayMidpointAtMinSensitivity, TravelDistanceUtilityDecayMidpointAtMaxSensitivity, _tuning.TravelDistanceSensitivity);
        _travelDistanceUtilitySteepness = Mathf.Lerp(TravelDistanceUtilitySteepnessAtMinSensitivity, TravelDistanceUtilitySteepnessAtMaxSensitivity, _tuning.TravelDistanceSensitivity);
    }

    #endregion

}
