using UnityEngine;

public class FinishedTurnState : TurnState
{
    public FinishedTurnState(MonoBehaviour manager) : base(manager)
    {
    }

    public override TurnStateType State => TurnStateType.Finished;

    public override void StartState(Character currentCharacter)
    {
        base.StartState(currentCharacter);
        EndState();
    }
}
