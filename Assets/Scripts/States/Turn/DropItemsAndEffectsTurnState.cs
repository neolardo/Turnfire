using UnityEngine;

public class DropItemsAndEffectsTurnState : TurnState
{
    private DropZone _dropZone;
    public DropItemsAndEffectsTurnState(MonoBehaviour manager, DropZone dropZone) : base(manager)
    {
        _dropZone = dropZone;
    }

    public override TurnStateType State => TurnStateType.DropItemsAndEffects;

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        _dropZone.AllDropsLanded += EndState;
    }

    protected override void UnsubscribeFromEvents()
    {
        base.UnsubscribeFromEvents();
        _dropZone.AllDropsLanded -= EndState;
    }

    public override void StartState()
    {
        base.StartState();
        _dropZone.SpawnDrops();
    }

}
