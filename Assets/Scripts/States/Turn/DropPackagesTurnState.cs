public class DropPackagesTurnState : TurnState
{
    public DropPackagesTurnState() : base(CoroutineRunner.Instance)
    {
    }

    public override TurnStateType State => TurnStateType.DropItemsAndEffects;

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        GameServices.DropManager.AllPackagesLanded += EndState;
    }

    protected override void UnsubscribeFromEvents()
    {
        base.UnsubscribeFromEvents();
        GameServices.DropManager.AllPackagesLanded -= EndState;
    }

    public override void StartState(TurnStateContext context)
    {
        base.StartState(context);
        GameServices.DropManager.TrySpawnPackages();
    }

}
