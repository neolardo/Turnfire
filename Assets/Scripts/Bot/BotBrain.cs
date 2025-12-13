using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UnityEditor;
using UnityEngine;

public class BotBrain : UnityDriven
{
    private BotTuningDefinition _tuning;

    public event Action<BotGoal> GoalDecided;

    // stationary point
    private Dictionary<Character, StandingPoint> _lastStandingPointPerCharacter;
    private Dictionary<Character,int> _numRecentStationaryTurnsPerCharacter;
    private const float StationaryPointScoreDecrementWeight = .5f;

    // utility constants
    private const float DamageUtilitySoftBoxTemperature = 0.06f;
    private const float PackageJumpDistanceUtilityExponentBase = 0.8f;
    private const float DamageCenterDistanceUtilityExponentBase = 0.45f;
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
        _lastStandingPointPerCharacter = new Dictionary<Character, StandingPoint>();
        _numRecentStationaryTurnsPerCharacter = new Dictionary<Character, int>();
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
        var scores = new List<float>();

        foreach (var targetPoint in possiblePositions)
        {
            scores.Add(EvaluatePositionScore(startPoint, targetPoint, context));
        }

        var pickedPoint = startPoint;
        if(possiblePositions.Any())
        {
            pickedPoint = PickBySoftmax(possiblePositions.ToList(), scores, _tuning.PositionDecisionSoftboxTemperature).item;
        }
        else
        {
            Debug.Log("Random point");
            pickedPoint = context.JumpGraph.GetRandomStandingPoint();
        }

        //stationary point update
        if (_lastStandingPointPerCharacter.ContainsKey(context.Self) && _lastStandingPointPerCharacter[context.Self] == pickedPoint)
        {
            _numRecentStationaryTurnsPerCharacter[context.Self]++;
        }
        else
        {
            _numRecentStationaryTurnsPerCharacter[context.Self] = 0;
            _lastStandingPointPerCharacter[context.Self] = pickedPoint;
        }

        if (pickedPoint == startPoint)
        {
            Debug.Log("Stay at position");
            onDone?.Invoke(BotGoal.SkipAction());
        }
        else
        {
            Debug.Log("Go to destination");
            onDone?.Invoke(BotGoal.Move(pickedPoint));
        }
        yield return null;
    }


    private float EvaluatePositionScore(StandingPoint startPoint, StandingPoint targetPoint, BotContext context)
    {
        // debug helpers TODO: delete
        Color travelColor = Color.blue;
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

        var weaponsWithAmmo = context.Self.GetAllItems().Where(i => i.Definition.ItemType == ItemType.Weapon && !i.Definition.IsQuantityInfinite);
        int remainingTotalAmmo = weaponsWithAmmo.Any() ? weaponsWithAmmo.Sum(w => w.Quantity): 0;

        // offense
        if(remainingTotalAmmo > 0)
        {
            var closestEnemyDistance = context.Enemies.Select(e => Vector2.Distance(targetPoint.WorldPos, e.transform.position)).DefaultIfEmpty(float.PositiveInfinity).Min();
            score += OffensiveEnemyDistanceUtility(closestEnemyDistance) * _tuning.Offense;

            newPos = lastPos + Vector2.up * scoreScaler * (score - lastScore);
            Debug.DrawLine(lastPos, newPos, offenseColor, raySeconds);
            lastPos = newPos;
            lastScore = score;
        }

        // defense
        if (context.Self.NormalizedHealth < _tuning.LowHealthThreshold)
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

        //travel distance
        if (context.JumpGraph.TryCalculateJumpDistanceBetween(startPoint, targetPoint, out int travelDistance))
        {
            score += TravelDistanceUtility(travelDistance) * _tuning.TravelDistanceWeight;

            newPos = lastPos + Vector2.up * scoreScaler * (score - lastScore);
            Debug.DrawLine(lastPos, newPos, travelColor, raySeconds);
            lastPos = newPos;
            lastScore = score;
        }

        // decision randomness
        score += GetDecisionRandomnessBias();

        newPos = lastPos + Vector2.up * scoreScaler * (score - lastScore);
        Debug.DrawLine(lastPos, newPos, randomColor, raySeconds);
        lastPos = newPos;
        lastScore = score;

        // stationary points
        if (_lastStandingPointPerCharacter.ContainsKey(context.Self) && targetPoint == _lastStandingPointPerCharacter[context.Self])
        {
            score -= _numRecentStationaryTurnsPerCharacter[context.Self] * StationaryPointScoreDecrementWeight;
        }

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


    #endregion

    #region Shooting

    private IEnumerator DecideGoalWhenReadyToUseItem(BotContext context, Action<BotGoal> onDone)
    {
        var items = context.Self.GetAllItems().Where(i=> i.Behavior.CanUseItem(new ItemUsageContext(context.Self)));
        var scores = new List<float>();
        var goals = new List<BotGoal>();

        foreach (var item in items)
        {
            float score = float.NegativeInfinity;
            BotGoal goal = BotGoal.SkipAction();
            yield return EvaluateItemScore(item, context, (s, g) => { score = s; goal = g; });
            goals.Add(goal);
            scores.Add(score);
            //yield return null;
        }

        var pickedGoal = BotGoal.SkipAction();
        if(items.Any())
        {
            pickedGoal = PickBySoftmax(goals, scores, _tuning.ItemDecisionSoftboxTemperature).item;
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
            score += HealingUtility(result.TotalHealingDone) * _tuning.Defense;
            score += ArmorUtility(result.TotalArmorBoost) * _tuning.Defense;
            score += MobilityUtility(result.TotalMobilityBoost) * _tuning.MobilityPreference;
            onDone?.Invoke(score, BotGoal.UseItem(item));
        }
    }

    public IEnumerator DecideAttackVector(Vector2 start, WeaponBehavior weaponBehavior, BotContext context, Action<Vector2, float> onDone)
    {
        var others = context.TeamMates.Concat(context.Enemies);
        var scores = new List<float>();
        var attackVectors = new List<Vector2>();

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
                scores.Add(score);
                attackVectors.Add(aimVector);

                if(weaponBehavior.IsAimingNormalized)
                {
                    break;
                }
            }
            //yield return null;
        }
        var pick = PickBySoftmax(attackVectors, scores, _tuning.AimDecisionSoftboxTemperature);
        onDone?.Invoke(pick.item, pick.score);
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
