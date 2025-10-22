using UnityEngine;

public class DropItemsAndEffectsTurnState : TurnState
{
    private DropManager _dropManager;
    public DropItemsAndEffectsTurnState(MonoBehaviour manager, DropManager dropManager) : base(manager)
    {
        _dropManager = dropManager;
    }

    public override TurnStateType State => TurnStateType.DropItemsAndEffects;

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        _dropManager.AllPackagesLanded += EndState;
    }

    protected override void UnsubscribeFromEvents()
    {
        base.UnsubscribeFromEvents();
        _dropManager.AllPackagesLanded -= EndState;
    }

    public override void StartState()
    {
        base.StartState();
        _dropManager.SpawnPackages();
    }

}
