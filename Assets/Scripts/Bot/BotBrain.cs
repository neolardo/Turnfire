using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BotBrain : UnityDriven
{
    private BotTuningDefinition _tuning;

    public event Action<BotGoal> GoalDecided;

    // utility constants
    private const float DamageUtilitySoftBoxTemperature = 0.06f;
    private const float PackageJumpDistanceUtilityExponentBase = 0.8f;
    private const float DamageCenterDistanceUtilityExponentBase = 0.55f;
    private const float OffenseUtilityDecayMidpoint = 4f;
    private const float OffenseUtilitySteepness = 1.5f;
    private const float DefenseUtilityDecayMidpoint = 5f;
    private const float DefenseUtilitySteepness = -1f;
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
        var closestEnemyDistance = context.Enemies.Min(e => Vector2.Distance(targetPoint.WorldPos, e.transform.position));
        score += OffenseUtility(closestEnemyDistance) * _tuning.OffensivePositionPreference;

        // defense
        if (context.Self.NormalizedHealth < _tuning.LowHealhThreshold)
        {
            var avarageEnemyDistance = context.Enemies.Average(e => Vector2.Distance(targetPoint.WorldPos, e.transform.position));
            score += DefenseUtility(avarageEnemyDistance) * _tuning.DefensivePositionPreference;
        }

        // package
        float bestPackageScore = 0;
        var weapons = context.Self.GetAllItems().Where(i => i.Definition.ItemType == ItemType.Weapon);
        int remainingTotalAmmo = weapons.Sum(i => i.Quantity);
        float packageSearchWeight = _tuning.GeneralPackageSearchWeight;
        if (remainingTotalAmmo == 0)
        {
            packageSearchWeight = _tuning.OutOfAmmoPackageSearchWeight;
        }
        else if (remainingTotalAmmo <= _tuning.RemainingAmmoLowThreshold)
        {
            packageSearchWeight = _tuning.RemainingAmmoLowPackageSearchWeight;
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
        score += bestPackageScore * packageSearchWeight;

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
        var selectedItem = context.Self.GetSelectedItem();
        var weapons = context.Self.GetAllItems().Where(i => i.Definition.ItemType == ItemType.Weapon);
        var startPos = context.Self.transform.position;
        if (selectedItem != null && weapons.Any())
        {
            float bestDamageScore = float.NegativeInfinity;
            Vector2 bestShootVector = default;
            var preferredWeapon = selectedItem.Definition.ItemType == ItemType.Weapon ? selectedItem : weapons.FirstOrDefault();
            if (_tuning.PreferHighestDamageDealingWeapon)
            {
                float damageScore = 0;
                Vector2 shootVector = default;
                foreach (var weapon in weapons)
                {
                    var weaponBehavior = weapon.Behavior as WeaponBehavior;
                    yield return CalculateBestShootVector(startPos, weaponBehavior, context, (vec, s) => { damageScore = s; shootVector = vec; });
                    if (damageScore > bestDamageScore)
                    {
                        bestDamageScore = damageScore;
                        bestShootVector = shootVector;
                        preferredWeapon = weapon;
                    }
                }
            }
            else
            {
                var weaponBehavior = preferredWeapon.Behavior as WeaponBehavior;
                yield return CalculateBestShootVector(startPos, weaponBehavior, context, (vec, s) => { bestDamageScore = s; bestShootVector = vec; });
            }
            if (!_tuning.OnlyShootIfCanDealDamage || bestDamageScore > 0)
            {
                var aimVector = ApplyAimRandomness(bestShootVector);
                onDone?.Invoke(BotGoal.Attack(aimVector, preferredWeapon));
            }
            else
            {
                onDone?.Invoke(BotGoal.SkipAction());
            }
        }
        else
        {
            onDone?.Invoke(BotGoal.SkipAction());
        }
    }

    public IEnumerator CalculateBestShootVector(Vector2 start, WeaponBehavior weaponBehavior, BotContext context, Action<Vector2, float> onDone)
    {
        float bestScore = float.NegativeInfinity;
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

                var minDistFromClosestEnemy = context.Enemies.Min(c => Vector2.Distance(c.transform.position, simulationResult.DamageCenter));

                float score = DamageUtility(simulationResult.TotalDamageDealtToEnemies) * _tuning.DamageDealtToEnemiesWeight;
                score += DamageUtility(simulationResult.TotalDamageDealtToAllies) * _tuning.DamageDealtToAlliesWeight;
                score += DamageCenterDistanceUtility(minDistFromClosestEnemy) * _tuning.DamageCenterDistanceFromClosestEnemyWeight;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestShot = aimVector;
                }

                if(weaponBehavior.IsAimingNormalized)
                {
                    break;
                }
            }
            yield return null;
        }

        onDone?.Invoke(bestShot, bestScore);
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

    private float OffenseUtility(float closestEnemyDistance)
    {
        return MathHelpers.Sigmoid(closestEnemyDistance, OffenseUtilityDecayMidpoint, OffenseUtilitySteepness);
    }

    private float DefenseUtility(float avarageEnemyDistance)
    {
        return MathHelpers.Sigmoid(avarageEnemyDistance, DefenseUtilityDecayMidpoint, DefenseUtilitySteepness);
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
