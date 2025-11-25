using System;
using System.Collections;
using System.Linq;
using System.Net;
using TMPro.EditorUtilities;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BotBrain : UnityDriven
{
    private BotTuning _tuning;
    public BotGameplayInput _input;

    public BotBrain(BotTuning tuning, BotGameplayInput input, MonoBehaviour coroutineManager) : base(coroutineManager)
    {
        _tuning = tuning;
        _input = input;
    }
    
    public void ThinkAndAct(BotContext context)
    {
        var goal = DecideGoal(context);
        Act(goal, context);
    }

    #region Plan

    private BotGoal DecideGoal(BotContext context)
    {
        switch (context.ActionState)
        {
            case CharacterActionStateType.ReadyToMove:
                return DecideGoalWhenReadyToMove(context);
            case CharacterActionStateType.ReadyToUseItem:
                return DecideGoalWhenReadyToUseItem(context);
            default:
                throw new System.Exception($"Invalid {nameof(CharacterActionStateType)} when planning bot goal: {context.ActionState}");
        }
    }

    private BotGoal DecideGoalWhenReadyToMove(BotContext context)
    {
        var startPos = context.JumpGraph.FindClosestStandingPoint(context.Self.FeetPosition);
        var possiblePositions = context.JumpGraph.GetAllLinkedStandingPointsFromStartPosition(startPos);
        possiblePositions = possiblePositions.Append(startPos);

        float bestScore = float.NegativeInfinity;
        StandingPoint bestPos = default;
        
        foreach(var pos in possiblePositions) 
        {
            var score = EvaluatePositionScore(pos, context);
            if(score > bestScore)
            {
                bestScore = score;
                bestPos = pos;
            }
        }

        if(bestPos == startPos)
        {
            return new BotGoal(BotGoalType.SkipAction);
        }
        else 
        {
            return new BotGoal(BotGoalType.MoveToTarget, bestPos);
        }
    }

    private IEnumerator EvaluatePositionScore(StandingPoint pos, BotContext context, Action<float> onDone)
    {
        float score = 0;
        // offense
        var selectedItem = context.Self.GetSelectedItem();
        Item preferredWeapon = null;
        if (selectedItem != null) 
        {
            var weapons = context.Self.GetAllItems().Where(i => i.Definition.ItemType == ItemType.Weapon);
            preferredWeapon = selectedItem.Definition.ItemType == ItemType.Weapon ? selectedItem : weapons.FirstOrDefault();
            float bestDamageScore = 0;
            if (_tuning.PreferHighestDamageDealingWeapon)
            {
                float damageScore = 0;
                foreach (var weapon in weapons)
                {
                    var weaponBehavior = (weapon.Behavior as WeaponBehavior);
                    yield return CalculateBestShootVector(pos.WorldPos - context.Self.FeetOffset, weaponBehavior, context, (_, s) => damageScore = s);
                    if( damageScore > bestDamageScore)
                    {
                        preferredWeapon = weapon;
                        bestDamageScore = damageScore;
                    }
                }
            }
            else
            {
                var weaponBehavior = (preferredWeapon.Behavior as WeaponBehavior);
                yield return CalculateBestShootVector(pos.WorldPos - context.Self.FeetOffset, weaponBehavior, context, (_, s) => bestDamageScore = s);
            }
            score += _tuning.BestDamagingPlaceSearchWeight * bestDamageScore; 
        }
        // defense
        if(context.Self.NormalizedHealth < _tuning.SafePlaceSearchHealthThreshold)
        {
            score -= _tuning.SafePlaceSearchWeight * NumEnemiesVisibleFromPoint(pos);
        }
        // package
        float bestPackageScore = 0;
        foreach (Package p in context.Packages)
        {
            var packagePos = context.JumpGraph.FindClosestStandingPoint(p.transform.position);
            if(context.JumpGraph.TryCalculateJumpDistanceBetween(pos, packagePos, out var numJumps))
            {
                float packageScore = numJumps * _tuning.PackageSearchWeight;
                if(packageScore > bestPackageScore) 
                {
                    bestPackageScore = packageScore;
                }
            }
        }
        score += bestPackageScore;
        // decision randomness
        score += UnityEngine.Random.Range(-_tuning.DecisionRandomnessBias/2f, _tuning.DecisionRandomnessBias/2f);
        //TODO: preferred weapon!
        onDone?.Invoke(score);
    }

    public IEnumerator CalculateBestShootVector(Vector2 start, WeaponBehavior weaponBehavior, BotContext context, Action<Vector2, float> onDone)
    {
        float bestScore = float.NegativeInfinity;
        Vector2 bestShot = default;
        var others = context.TeamMates.Concat(context.Enemies);

        for (float angle = 0; angle < 360f; angle += Constants.AimAngleSimulationStep)
        {
            Vector2 direction = angle.AngleDegreesToVector();
            for (float strength = 0; strength <= 1f; strength += Constants.AimStrengthSimulationStep)
            {
                Vector2 aimVector = direction * strength;

                var result = weaponBehavior.SimulateWeaponBehavior(start, aimVector, context.Terrain, context.Self, others);

                float score = 
                if (score > bestScore)
                {
                    bestScore = score;
                    bestShot = aimVector;
                }
            }
            yield return null;
        }

        onDone?.Invoke(bestShot, bestScore);
    }

    private int NumEnemiesVisibleFromPoint(StandingPoint pos)
    {
        //TODO: implement
        return 0;
    }

    private BotGoal DecideGoalWhenReadyToUseItem(BotContext context)
    {
        // damage to enemies?
        // damage to allies?
        // ammo?
        // best weapon?
        // + aim randomness
        // + decision randomness

        if (context.Self.GetSelectedItem() == null)
        {
            return new BotGoal(BotGoalType.SkipAction);
        }
        else
        {
            return new BotGoal(BotGoalType.ShootTarget, context.Enemies.First().transform.position, context.Self.GetSelectedItem());
        }
    }

    #endregion

    #region Act

    private void Act(BotGoal goal, BotContext context)
    {
        switch(goal.GoalType)
        {
            case BotGoalType.MoveToTarget:
                MoveToTarget(goal.Target, context);
                Debug.Log("Bot moved to target");
                break;
            case BotGoalType.ShootTarget:
                ShootTarget(goal.Target, goal.PreferredItem, context);
                Debug.Log("Bot shot");
                break;
            case BotGoalType.UseItem:
                UseItem(goal.PreferredItem, context);
                Debug.Log("Bot used item");
                break;
            case BotGoalType.SkipAction:
                Debug.Log("Bot skipped action");
                SkipAction();
                break;
            default:
                throw new System.Exception($"Invalid {nameof(BotGoalType)} when trying to act with a bot: {goal.GoalType}");
        }
    }

    #region Move

    private void MoveToTarget(Vector2 target, BotContext context)
    {
        var start = context.Self.FeetPosition;
        var jumpPath = context.JumpGraph.FindShortestJumpPath(start, target);
        if (jumpPath == null || jumpPath.Count == 0) //TODO: why? where to jump instead?
        {
            _input.SkipAction();
        }
        else
        {
            var jumpLink = jumpPath.First();
            if (context.JumpGraph.IsJumpPredictionValid(start, jumpLink, context.Terrain))
            {
                _input.AimAndRelease(jumpLink.JumpVector);
            }
            else
            {
                var jumpVector = context.JumpGraph.CalculateCorrectedJumpVectorToStandingPoint(start, jumpLink.FromId);
                _input.AimAndRelease(jumpVector);
            }
        }
    }


    #endregion

    #region Shoot

    private void ShootTarget(Vector2 target, Item weapon, BotContext context)
    {
        _input.SwitchSelectedItemTo(weapon);
        StartCoroutine(CalculateShootVectorAndFireAtTarget(target, weapon, context));
    }

    private IEnumerator CalculateShootVectorAndFireAtTarget(Vector2 target, Item weapon, BotContext context)
    {
        Vector2 aimVector = default;
        yield return CalculateShootVector(context.Self.transform.position, target, weapon.Behavior as WeaponBehavior, context, vec => aimVector = vec);
        //TODO: apply randomness
        (weapon.Behavior as WeaponBehavior).SimulateWeaponBehavior(context.Self.transform.position, aimVector, context.Terrain, context.Self, context.Enemies.Concat(context.TeamMates), true);
        _input.AimAndRelease(aimVector);
    }

   

    #endregion

    private void UseItem(Item item, BotContext context)
    {
        _input.SwitchSelectedItemTo(item);
        _input.UseSelectedItem();
    }

    private void SkipAction()
    {
        _input.SkipAction();
    }

    #endregion
}
