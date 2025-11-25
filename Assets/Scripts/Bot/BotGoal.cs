using UnityEngine;

public readonly struct BotGoal
{
    public readonly BotGoalType GoalType;
    public readonly Vector2 Target;
    public readonly StandingPoint TargetPoint;
    public readonly Item PreferredItem;

    public BotGoal(BotGoalType goal)
    {
        GoalType = goal;
        Target = default;
        TargetPoint = default;
        PreferredItem = default;
    }
    public BotGoal(BotGoalType goal, Vector2 target)
    {
        GoalType = goal;
        Target = target;
        TargetPoint = default;
        PreferredItem = default;
    }
    public BotGoal(BotGoalType goal, StandingPoint point)
    {
        GoalType = goal;
        Target = point.WorldPos;
        TargetPoint = point;
        PreferredItem = default;
    }

    public BotGoal(BotGoalType goal, Vector2 target, Item preferredItem)
    {
        GoalType = goal;
        Target = target;
        TargetPoint = default;
        PreferredItem = preferredItem;
    }
    public BotGoal(BotGoalType goal, Item preferredItem)
    {
        GoalType = goal;
        Target = default;
        TargetPoint = default;
        PreferredItem = preferredItem;
    }
}
