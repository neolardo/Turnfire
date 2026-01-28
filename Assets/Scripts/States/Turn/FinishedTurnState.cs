public class FinishedTurnState : TurnState
{
    public FinishedTurnState() : base(CoroutineRunner.Instance)
    {
    }

    public override TurnStateType State => TurnStateType.Finished;

    public override void StartState(TurnStateContext context)
    {
        base.StartState(context);
        EndState();
    }

}
