using UnityEngine;

public abstract class CharacterActionState : StateBase
{
    public abstract CharacterActionStateType State { get; }
    protected Character _currentCharacter;
    protected UISoundsDefinition _uiSounds;

    protected CharacterActionState(MonoBehaviour coroutineManager, UISoundsDefinition uiSounds) : base(coroutineManager)
    {
        _uiSounds = uiSounds;
    }

    public virtual void StartState(Character currentCharacter)
    {
        IsActive = true;
        _currentCharacter = currentCharacter;
        SubscribeToEvents();
    }

    protected virtual void EndState()
    {
        IsActive = false;
        UnsubscribeFromEvents();
        InvokeStateEndedEvent();
    }

    protected virtual void OnActionSkipped()
    {
        AudioManager.Instance.PlayUISound(_uiSounds.SkipAction);
        EndState();
    }

    public void ForceEndState()
    {
        EndState();
    }
}
