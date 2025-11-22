using UnityEngine;

public readonly struct BotGoal
{
    public readonly BotGoalType GoalType;
    public readonly Vector2 Target;
    public readonly Item PreferredItem;

    public BotGoal(BotGoalType goal)
    {
        GoalType = goal;
        Target = default;
        PreferredItem = default;
    }
    public BotGoal(BotGoalType goal, Vector2 target)
    {
        GoalType = goal;
        Target = target;
        PreferredItem = default;
    }
    public BotGoal(BotGoalType goal, Vector2 target, Item preferredItem)
    {
        GoalType = goal;
        Target = target;
        PreferredItem = preferredItem;
    }
    public BotGoal(BotGoalType goal, Item preferredItem)
    {
        GoalType = goal;
        Target = default;
        PreferredItem = preferredItem;
    }
}
