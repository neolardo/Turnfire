using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public class BotBrain : UnityDriven
{
    private BotTuningDefinition _tuning;
    private Dictionary<Character, BotPositionMemory> _positionMemory;

    public event Action<BotGoal> GoalDecided;

    // utility constants
    private const float DamageUtilitySoftmaxTemperature = 0.06f;
    private const float PackageJumpDistanceUtilityExponentBase = 0.65f;
    private const float DamageCenterDistanceUtilityExponentBase = 0.45f;

    private const float OffensiveRangedEnemyDistanceUtilityRiseDecayMidpoint = 0.8f;
    private const float OffensiveRangedEnemyDistanceUtilityRiseSteepness = -4f;
    private const float OffensiveRangedEnemyDistanceUtilityRiseFallMidpoint = 2f;
    private const float OffensiveRangedEnemyDistanceUtilityFallDecayMidpoint = 5f;
    private const float OffensiveRangedEnemyDistanceUtilityFallSteepness = 1.5f;

    private const float OffensiveMeleeEnemyDistanceUtilityDecayMidpoint = 1.2f;
    private const float OffensiveMeleeEnemyDistanceUtilitySteepness = 3.4f;

    private const float DefensiveEnemyDistanceUtilityDecayMidpoint = 5f;
    private const float DefensiveEnemyDistanceUtilitySteepness = -1f;

    private const float MobilityUtilityDecayMidpoint = 1.5f;
    private const float MobilityUtilitySteepness = -2f;

    private const float HealingUtilityDecayMidpoint = 2f;
    private const float HealingUtilitySteepness = -0.7f;

    private const float ArmorUtilityDecayMidpoint = 1.7f;
    private const float ArmorUtilitySteepness = -1.1f;

    // position memory constants
    private const int PositionMemorySize = 5;
    private const float PositionMemoryPenaltyPerTurn = .3f;
    private const float PositionMemoryStationaryPointDistance = 1f;

    public BotBrain(BotTuningDefinition tuning, MonoBehaviour coroutineManager) : base(coroutineManager)
    {
        _tuning = tuning;
        _positionMemory = new Dictionary<Character, BotPositionMemory>();
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
        InitializeBotMemory(context.Self);

        var startPoint = context.JumpGraph.FindClosestStandingPoint(context.Self.FeetPosition);
        var possibleTargetPoints = context.JumpGraph.GetAllReachableStandingPointsFromPoint(startPoint).ToList();

        var scores = new List<float>();

        foreach (var targetPoint in possibleTargetPoints)
        {
            var score = EvaluatePositionScore(targetPoint, context);
            scores.Add(score);
        }

        Debug.Log($"Reachable points count: {possibleTargetPoints.Count}");
        var pickedPoint = possibleTargetPoints.Count == 0 ? StandingPoint.InvalidPoint : PickBySoftmax(possibleTargetPoints, scores, _tuning.PositionDecisionSoftboxTemperature).item;
        if (pickedPoint == StandingPoint.InvalidPoint)
        {
            Debug.Log("No positions to pick from. Picked a random jump vector.");
            onDone?.Invoke(BotGoal.Move(GetRandomJumpVector()));
        }
        else if (pickedPoint == startPoint)
        {
            MemorizePoint(context.Self, pickedPoint);
            Debug.Log("Already at the destination.");
            onDone?.Invoke(BotGoal.SkipAction());
        }
        else
        {
            MemorizePoint(context.Self, pickedPoint);
            var jumpStrength = context.Self.JumpStrength;
            var firstJumpInPath = context.JumpGraph.FindShortestJumpPath(startPoint, pickedPoint).First();
            var correctedJumpVector = context.JumpGraph.CorrectJumpVector(context.Self.FeetPosition, firstJumpInPath, context.Terrain);
            onDone?.Invoke(BotGoal.Move(correctedJumpVector / jumpStrength));
            Debug.Log("Moved to the destination.");
        }
        yield return null;
    }

    private static Vector2 GetRandomJumpVector()
    {
        var randVec = UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0, 1);
        return new Vector2(randVec.x, MathF.Abs(randVec.y));
    }

    private float EvaluatePositionScore(StandingPoint targetPoint, BotContext context)
    {
        // debug helpers TODO: delete
        Color offenseColor = Color.red;
        Color defenseColor = Color.yellow;
        Color packageColor = Color.green;
        Color randomColor = Color.gray;
        Color stationaryColor = Color.magenta;
        float scoreScaler = 0.33f;
        float raySeconds = 40;
        float lastScore = 0;
        Vector2 lastPos = targetPoint.WorldPos;
        Vector2 newPos = lastPos;

        float score = 0;

        var weapons = context.Self.GetAllItems().Where(i => i.Definition.ItemType == ItemType.Weapon);
        var remainingTotalAmmo = weapons.Where(w => !w.Definition.IsQuantityInfinite).Select(w => w.Quantity).DefaultIfEmpty(0).Sum();
        bool hasRangedWeapons = weapons.Any(w => (w.Definition as WeaponDefinition).IsRanged);

        // offense
        if (weapons.Any())
        {
            var closestEnemyDistance = context.Enemies.Select(e => Vector2.Distance(targetPoint.WorldPos, e.transform.position)).DefaultIfEmpty(float.PositiveInfinity).Min();
            score += OffensiveEnemyDistanceUtility(closestEnemyDistance, hasRangedWeapons) * _tuning.Offense;

            newPos = lastPos + Vector2.up * scoreScaler * (score - lastScore);
            Debug.DrawLine(lastPos, newPos, offenseColor, raySeconds);
            lastPos = newPos;
            lastScore = score;
        }

        // defense
        if (context.Self.Health < _tuning.LowHealthThreshold)
        {
            var avarageEnemyDistance = context.Enemies.Select(e => Vector2.Distance(targetPoint.WorldPos, e.transform.position)).DefaultIfEmpty(float.PositiveInfinity).Average();
            score += DefensiveEnemyDistanceUtility(avarageEnemyDistance) * _tuning.Defense;

            newPos = lastPos + Vector2.up * scoreScaler * (score - lastScore);
            Debug.DrawLine(lastPos, newPos, defenseColor, raySeconds);
            lastPos = newPos;
            lastScore = score;
        }

        // package
        float bestPackageScore = 0;
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

        newPos = lastPos + Vector2.up * scoreScaler * (score - lastScore);
        Debug.DrawLine(lastPos, newPos, packageColor, raySeconds);
        lastPos = newPos;
        lastScore = score;

        // decision randomness
        score += GetDecisionRandomnessBias();

        newPos = lastPos + Vector2.up * scoreScaler * (score - lastScore);
        Debug.DrawLine(lastPos, newPos, randomColor, raySeconds);
        lastPos = newPos;
        lastScore = score;

        // stationary points
        score -= GetStationaryPenalty(context.Self, targetPoint);

        newPos = lastPos + Vector2.up * scoreScaler * (score - lastScore);
        Debug.DrawLine(lastPos, newPos, stationaryColor, raySeconds);
        lastPos = newPos;
        lastScore = score;


        return score;
    }

    private float GetDecisionRandomnessBias()
    {
        return UnityEngine.Random.Range(-_tuning.DecisionJitterBias / 2f, _tuning.DecisionJitterBias / 2f);
    }

    #region Bot Position Memory

    private void InitializeBotMemory(Character character)
    {
        if (!_positionMemory.ContainsKey(character))
        {
            _positionMemory[character] = new BotPositionMemory(PositionMemoryPenaltyPerTurn, PositionMemorySize, PositionMemoryStationaryPointDistance);
        }
    }

    private void MemorizePoint(Character character, StandingPoint point)
    {
        _positionMemory[character].Commit(point);
    }

    private float GetStationaryPenalty(Character character, StandingPoint point)
    {
        return _positionMemory[character].GetStationaryPenalty(point);
    }


    #endregion

    #endregion

    #region Shooting

    private IEnumerator DecideGoalWhenReadyToUseItem(BotContext context, Action<BotGoal> onDone)
    {
        var itemUsageContext = new ItemUsageContext(context.Self);
        var useableItems = context.Self.GetAllItems().Where( i => i.Behavior.CanUseItem(new ItemUsageContext(context.Self)));
        float bestScore = float.NegativeInfinity;
        BotGoal bestGoal = BotGoal.SkipAction();


        foreach (var item in useableItems)
        {
            float score = float.NegativeInfinity;
            BotGoal goal = BotGoal.SkipAction();
            yield return EvaluateItemScore(item, context, (s, g) => { score = s; goal = g; });
            if (score > bestScore)
            { 
                bestGoal = goal;
                bestScore = score;
            }
            //yield return null;
        }

        var pickedGoal = BotGoal.SkipAction();
        if(useableItems.Any())
        {
            pickedGoal = bestGoal;//PickBySoftmax(goals, scores, _tuning.ItemDecisionSoftboxTemperature).item;
        }

        onDone?.Invoke(pickedGoal);       
    }

    private IEnumerator EvaluateItemScore(Item item, BotContext context, Action<float, BotGoal> onDone)
    {
        var startPos = context.Self.ItemTransform.position;
        var simulationContext = new ItemBehaviorSimulationContext(context.Self, context.TeamMates.Concat(context.Enemies), startPos, Vector2.zero, context.Terrain);
        float score = float.NegativeInfinity;
        if (item.Definition.ItemType == ItemType.Weapon)
        {
            var weaponBehavior = item.Behavior as WeaponBehavior;
            Vector2 attackVector = default;
            yield return DecideAttackVector(startPos, weaponBehavior, context, (vec, s) => { score = s; attackVector = vec; });
            attackVector = ApplyAimRandomness(attackVector);
            onDone?.Invoke(score, BotGoal.Attack(attackVector, item));
        }
        else
        {
            var result = ItemBehaviorSimulationResult.None;
            yield return item.Behavior.SimulateUsage(simulationContext, (r) => result = r);
            score = HealingUtility(result.TotalHealingDone) * _tuning.Defense;
            score += ArmorUtility(result.TotalArmorBoost) * _tuning.Defense;
            score += MobilityUtility(result.TotalMobilityBoost) * _tuning.MobilityPreference;
            onDone?.Invoke(score, BotGoal.UseItem(item));
        }
    }

    public IEnumerator DecideAttackVector(Vector2 start, WeaponBehavior weaponBehavior, BotContext context, Action<Vector2, float> onDone)
    {
        var others = context.TeamMates.Concat(context.Enemies);
        var bestScore = float.NegativeInfinity;
        var bestAttackVector = Vector2.zero;

        for (float angle = -30; angle < 210f; angle += Constants.AimAngleSimulationStep) // only check valid shoot angles
        {
            Vector2 direction = angle.AngleDegreesToVector();
            for (float magnitude = Constants.AimStrengthSimulationStep; magnitude <= 1f; magnitude += Constants.AimStrengthSimulationStep)
            {
                Vector2 aimVector = direction * magnitude;

                var simulationContext = new ItemBehaviorSimulationContext(context.Self, others, start, aimVector, context.Terrain);
                var simulationResult = ItemBehaviorSimulationResult.None;

                if (weaponBehavior.FastSimAvailable)
                {
                    simulationResult = weaponBehavior.SimulateUsageFast(simulationContext);
                }
                else
                { 
                    yield return weaponBehavior.SimulateUsage(simulationContext, (result) => simulationResult = result);
                }
                float score = 0;
                
                if(Mathf.Approximately(simulationResult.TotalDamageDealtToEnemies, 0) && Mathf.Approximately(simulationResult.TotalDamageDealtToAllies, 0))
                {
                    var minDistFromClosestEnemy = context.Enemies.Select(c => Vector2.Distance(c.transform.position, simulationResult.DamageCenter)).DefaultIfEmpty(float.PositiveInfinity).Min();
                    score = DamageCenterDistanceUtility(minDistFromClosestEnemy) * _tuning.Offense;
                }
                else 
                {
                    score = DamageUtility(simulationResult.TotalDamageDealtToEnemies) * _tuning.Offense;
                    score -= DamageUtility(simulationResult.TotalDamageDealtToAllies) * _tuning.Defense;
                }

                if(score > bestScore)
                {
                    bestScore = score;
                    bestAttackVector = aimVector;
                }

                if(weaponBehavior.IsAimingNormalized)
                {
                    break;
                }
            }
            //yield return null;
        }
        onDone?.Invoke(bestAttackVector, bestScore);
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

    #region Softmax Pick

    private (T item, float score) PickBySoftmax<T>(IList<T> items, IList<float> scores, float temperature)
    {
        float max = scores.Max();

        float sum = 0f;
        var weights = new float[scores.Count];
        for (int i = 0; i < scores.Count; ++i)
        {
            float w = Mathf.Exp((scores[i] - max) / Mathf.Max(0.0001f, temperature)); // lower T -> more greedy, higher T -> more random
            weights[i] = w;
            sum += w;
        }

        if (sum <= 0f)
        {
            return (items[0], scores[0]);
        }

        float pick = UnityEngine.Random.value * sum;
        float acc = 0f;
        for (int i = 0; i < weights.Length; ++i)
        {
            acc += weights[i];
            if (pick <= acc)
            {
                return (items[i], scores[i]);
            }
        }
        return (items[items.Count - 1], scores[items.Count -1]);
    }

    #endregion

    #region Normalizing Utility Functions

    private float DamageUtility(float damage)
    {
        return 1f - Mathf.Exp(-damage * DamageUtilitySoftmaxTemperature);
    }

    private float OffensiveEnemyDistanceUtility(float closestEnemyDistance, bool isRangedDistance)
    {
        if (isRangedDistance)
        {
            if (closestEnemyDistance < OffensiveRangedEnemyDistanceUtilityRiseFallMidpoint)
            {
                return MathHelpers.Sigmoid(closestEnemyDistance, OffensiveRangedEnemyDistanceUtilityRiseDecayMidpoint, OffensiveRangedEnemyDistanceUtilityRiseSteepness);
            }
            else
            {
                return MathHelpers.Sigmoid(closestEnemyDistance, OffensiveRangedEnemyDistanceUtilityFallDecayMidpoint, OffensiveRangedEnemyDistanceUtilityFallSteepness);
            }
        }
        else
        {
            return MathHelpers.Sigmoid(closestEnemyDistance, OffensiveMeleeEnemyDistanceUtilityDecayMidpoint, OffensiveMeleeEnemyDistanceUtilitySteepness);
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

    #endregion

}
