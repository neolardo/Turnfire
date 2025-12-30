using System;
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
                Move(goal.AimVector);
                Debug.Log("Bot moved to destination");
                break;
            case BotGoalType.Attack:
                Attack(goal.AimVector, goal.PreferredItem);
                Debug.Log($"Bot attacked with {goal.PreferredItem.Definition.Name}");
                break;
            case BotGoalType.UseItem:
                UseItem(goal.PreferredItem, new ItemUsageContext(context.Self));
                Debug.Log($"Bot used item: {goal.PreferredItem.Definition.Name}");
                break;
            case BotGoalType.SkipAction:
                SkipAction();
                Debug.Log("Bot skipped action");
                break;
            default:
                throw new Exception($"Invalid {nameof(BotGoalType)} when trying to act with a bot: {goal.GoalType}");
        }
    }

    private void Move(Vector2 jumpVector)
    {
        _input.AimAndRelease(jumpVector);
    }

    private void Attack(Vector2 aimVector, Item weapon)
    {
        _input.SetSelectedItem(weapon);
        _input.AimAndRelease(aimVector);
    }

    private void UseItem(Item item, ItemUsageContext context)
    {
        _input.SetSelectedItem(item);
        if(!item.Definition.UseInstantlyWhenSelected)
        {
            _input.UseSelectedItem(context);
        }
    }
    private void SkipAction()
    {
        _input.SkipAction();
    }

}
