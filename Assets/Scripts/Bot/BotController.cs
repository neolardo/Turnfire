using System;
using UnityEngine;

public class BotController
{
    public event Action<Vector2> AimAndRelease; 
    public event Action<ItemInstance> SwitchSelectedItem; 
    public event Action<ItemUsageContext> UseSelectedItem;
    public event Action SkipAction;

    public void Act(BotGoal goal, BotContext context)
    {
        switch (goal.GoalType)
        {
            case BotGoalType.Move:
                ActMove(goal.AimVector);
                Debug.Log("Bot moved to destination");
                break;
            case BotGoalType.Attack:
                ActAttack(goal.AimVector, goal.PreferredItem);
                Debug.Log($"Bot attacked with {goal.PreferredItem.Definition.Name}");
                break;
            case BotGoalType.UseItem:
                ActUseItem(goal.PreferredItem, new ItemUsageContext(context.Self));
                Debug.Log($"Bot used item: {goal.PreferredItem.Definition.Name}");
                break;
            case BotGoalType.SkipAction:
                ActSkipAction();
                Debug.Log("Bot skipped action");
                break;
            default:
                throw new Exception($"Invalid {nameof(BotGoalType)} when trying to act with a bot: {goal.GoalType}");
        }
    }

    private void ActMove(Vector2 jumpVector)
    {
        AimAndRelease?.Invoke(jumpVector);
    }

    private void ActAttack(Vector2 aimVector, ItemInstance weapon)
    {
        SwitchSelectedItem?.Invoke(weapon);
        AimAndRelease?.Invoke(aimVector);
    }

    private void ActUseItem(ItemInstance item, ItemUsageContext context)
    {
        SwitchSelectedItem?.Invoke(item);
        if(!item.Definition.UseInstantlyWhenSelected)
        {
            UseSelectedItem?.Invoke(context);
        }
    }
    private void ActSkipAction()
    {
        SkipAction?.Invoke();
    }

}
