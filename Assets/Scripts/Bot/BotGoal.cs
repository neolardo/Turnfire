using UnityEngine;

public readonly struct BotGoal
{
    public readonly BotGoalType GoalType;
    public readonly Vector2 AimVector;
    public readonly ItemInstance PreferredItem;

    public static BotGoal SkipAction() => new BotGoal(BotGoalType.SkipAction, default, default);
    public static BotGoal Move(Vector2 jumpVector) => new BotGoal(BotGoalType.Move, jumpVector, default);
    public static BotGoal Attack(Vector2 attackVector, ItemInstance preferredItem) => new BotGoal(BotGoalType.Attack, attackVector, preferredItem);
    public static BotGoal UseItem(ItemInstance preferredItem) => new BotGoal(BotGoalType.UseItem, default, preferredItem);

    public BotGoal(BotGoalType goal, Vector2 aimVector, ItemInstance preferredItem)
    {
        GoalType = goal;
        AimVector = aimVector;
        PreferredItem = preferredItem;
    }
}
