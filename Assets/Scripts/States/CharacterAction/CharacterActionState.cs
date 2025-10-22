using UnityEngine;

public abstract class CharacterActionState : StateBase
{
    public abstract CharacterActionStateType State { get; }
    protected Character _currentCharacter;

    protected CharacterActionState(MonoBehaviour coroutineManager) : base(coroutineManager)
    {
    }

    public virtual void StartState(Character currentCharacter)
    {
        Debug.Log(State.ToString() + " started");
        IsActive = true;
        _currentCharacter = currentCharacter;
        SubscribeToEvents();
    }

    protected virtual void EndState()
    {
        Debug.Log(State.ToString() + " ended");
        IsActive = false;
        UnsubscribeFromEvents();
        InvokeStateEndedEvent();
    }

    public void ForceEndState()
    {
        EndState();
    }
}
