using UnityEngine;

public class FinishedTurnState : TurnState
{
    public FinishedTurnState(MonoBehaviour manager) : base(manager)
    {
    }

    public override TurnStateType State => TurnStateType.Finished;

    public override void StartState()
    {
        base.StartState();
        foreach (var data in BotEvaluationStatistics.GetAllData())
        {
            data.TotalTurnCount++;
        }
        EndState();
    }

}
