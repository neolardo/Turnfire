using System;
using System.Linq;
using UnityEngine;

public class BotController
{
    private BotGameplayInput _input;

    public BotController(BotGameplayInput input)
    {
        _input = input;
    }

    public void Act(BotGoal goal, BotContext context)
    {
        switch (goal.GoalType)
        {
            case BotGoalType.Move:
                Move(goal.Destination, context);
                Debug.Log("Bot moved to destination");
                break;
            case BotGoalType.Attack:
                Attack(goal.AttackVector, goal.PreferredItem);
                Debug.Log("Bot attacked");
                break;
            case BotGoalType.UseItem:
                UseItem(goal.PreferredItem);
                Debug.Log("Bot used item");
                break;
            case BotGoalType.SkipAction:
                Debug.Log("Bot skipped action");
                SkipAction();
                break;
            default:
                throw new Exception($"Invalid {nameof(BotGoalType)} when trying to act with a bot: {goal.GoalType}");
        }
    }

    private void Move(StandingPoint destinationPoint, BotContext context)
    {
        var feetPosition = context.Self.FeetPosition;
        var startPoint = context.JumpGraph.FindClosestStandingPoint(feetPosition);
        var jumpPath = context.JumpGraph.FindShortestJumpPath(startPoint, destinationPoint);
        if (jumpPath == null) // if not reachable jump with a random vector
        {
            var randomAimVector = UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0, 1);
            randomAimVector = new Vector2 (randomAimVector.x, MathF.Abs(randomAimVector.y));
            _input.AimAndRelease(randomAimVector);
        }
        else if (jumpPath.Count == 0) // already at the destination
        {
            _input.SkipAction();
        }
        else
        {
            var jumpLink = jumpPath.First();
            Vector2 jumpVector = jumpLink.JumpVector;
            if (!context.JumpGraph.IsJumpPredictionValid(feetPosition, jumpLink, context.Terrain))
            {
                jumpVector = context.JumpGraph.CalculateCorrectedJumpVectorToStandingPoint(feetPosition, startPoint);
            }
            _input.AimAndRelease(jumpVector / context.Self.JumpStrength);
        }
    }


    private void Attack(Vector2 aimVector, Item weapon)
    {
        _input.SwitchSelectedItemTo(weapon);
        _input.AimAndRelease(aimVector);
    }

    private void UseItem(Item item)
    {
        _input.SwitchSelectedItemTo(item);
        _input.UseSelectedItem();
    }

    private void SkipAction()
    {
        _input.SkipAction();
    }

}
