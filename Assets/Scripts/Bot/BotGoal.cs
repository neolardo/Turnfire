using UnityEngine;

public readonly struct BotGoal
{
    public readonly BotGoalType GoalType;
    public readonly Vector2 ShootVector;
    public readonly StandingPoint Destination;
    public readonly Item PreferredItem;

    public static BotGoal SkipAction() => new BotGoal(BotGoalType.SkipAction, default, default, default);
    public static BotGoal Move(StandingPoint destination) => new BotGoal(BotGoalType.Move, destination, default, default);
    public static BotGoal Shoot(Vector2 shootVector, Item preferredItem) => new BotGoal(BotGoalType.Shoot, default, shootVector, preferredItem);
    public static BotGoal UseItem(Item preferredItem) => new BotGoal(BotGoalType.UseItem, default, default, preferredItem);

    public BotGoal(BotGoalType goal, StandingPoint destination, Vector2 shootVecor, Item preferredItem)
    {
        GoalType = goal;
        ShootVector = shootVecor;
        Destination = destination;
        PreferredItem = preferredItem;
    }
}
