using UnityEngine;

public abstract class TurnState : StateBase
{
    public abstract TurnStateType State { get; }

    protected TurnState(MonoBehaviour coroutineManager) : base(coroutineManager)
    {
    }

    public virtual void StartState(TurnStateContext context)
    {
        Debug.Log(State.ToString() + " started");
        IsActive = true;
        SubscribeToEvents();
    }

    protected virtual void EndState()
    {
        IsActive = false;
        UnsubscribeFromEvents();
        InvokeStateEndedEvent();
    }
    public virtual void ForceEndState()
    {
        EndState();
    }
}

