using UnityEngine;

public class DropTurnState : TurnState
{
    private DropZone _dropZone;
    public DropTurnState(MonoBehaviour manager, DropZone dropZone) : base(manager)
    {
        _dropZone = dropZone;
    }

    public override TurnStateType State => TurnStateType.Drop;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        _dropZone.AllDropsLanded += EndState;
        _dropZone.SpawnDrops();
    }

    protected override void EndState()
    {
        base.EndState();
        _dropZone.AllDropsLanded -= EndState;
    }

}
