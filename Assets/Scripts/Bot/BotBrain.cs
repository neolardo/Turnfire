using System.Linq;
using UnityEngine;

public class BotBrain
{
    private BotTuning _tuning;
    public BotGameplayInput _input;

    public BotBrain(BotTuning tuning, BotGameplayInput input)
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
        if (context.Packages.Count() > 0)
        {
            return new BotGoal(BotGoalType.MoveToTarget, context.Packages.First().transform.position); //TODO: closest by jump
        }


        if (context.Self.NormalizedHealth < _tuning.FleeHealthThreshold || context.Self.GetSelectedItem() == null)
        {
            if(context.Packages.Count() > 0)
            {
                return new BotGoal(BotGoalType.MoveToTarget, context.Packages.First().transform.position); //TODO: closest by jump
            }
            else
            {
                return new BotGoal(BotGoalType.Flee);
            }
        }
        else
        {
            return new BotGoal(BotGoalType.SkipAction); //TODO
        }
    }

    private BotGoal DecideGoalWhenReadyToUseItem(BotContext context)
    {
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
            case BotGoalType.Flee:
                Flee(context);
                Debug.Log("Bot flee");
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
        if (jumpPath == null || jumpPath.Count == 0)
        {
            _input.SkipAction();
        }
        else
        {
            var jumpLink = jumpPath.First();
            if (context.JumpGraph.IsJumpPredictionValid(start, jumpLink, context.DestructibleTerrain))
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


    private void Flee(BotContext context)
    {
        var direction = Random.insideUnitCircle; //TODO: safe position instead
        _input.AimAndRelease(direction);
    }

    #endregion

    #region Shoot

    private void ShootTarget(Vector2 target, Item weapon, BotContext context)
    {
        _input.SwitchSelectedItemTo(weapon);
        var aimVector = CalculateShootVector(context.Self.transform.position, target, weapon.Behavior as WeaponBehavior, context);
        //TODO: apply randomness
        _input.AimAndRelease(aimVector);
    }

    //TODO: to coroutine
    public Vector2 CalculateShootVector(Vector2 start, Vector2 target, WeaponBehavior weaponBehavior, BotContext context)
    {
        float bestScore = float.NegativeInfinity;
        Vector2 bestShot = default;

        for (float angle = 0; angle < 360f; angle+= Constants.AimAngleSimulationStep)
        {
            Vector2 direction = angle.AngleDegreesToVector();
            for (float strength = 0; strength <= 1f; strength += Constants.AimStrengthSimulationStep)
            {
                Vector2 aimVector = direction * strength;

                var destination = weaponBehavior.SimulateWeaponBehaviorAndCalculateClosestPositionToTarget(start, target, aimVector, context.DestructibleTerrain, context.Self);
                float score = destination.Approximately(target) ? float.PositiveInfinity :  1f / Vector2.Distance(destination, target);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestShot = aimVector;
                }
            }
        }

        return bestShot;
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
