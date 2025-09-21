using System;
using System.Collections;
using UnityEngine;

public abstract class TurnState
{
    public abstract TurnStateType State { get; }
    public bool IsActive { get; protected set; }
    protected Character _currentCharacter;
    protected MonoBehaviour _coroutineManager;

    public Action TurnStateEnded;

    protected TurnState(MonoBehaviour coroutineManager)
    {
        _coroutineManager = coroutineManager;
    }

    protected Coroutine StartCoroutine(IEnumerator routine)
    {
        return _coroutineManager.StartCoroutine(routine);
    }

    public virtual void StartState(Character currentCharacter)
    {
        IsActive = true;
        SubscribeToEvents();
        _currentCharacter = currentCharacter;
    }

    protected virtual void EndState()
    {
        IsActive = false;
        UnsubscribeFromEvents();
        TurnStateEnded?.Invoke();
    }

    protected virtual void SubscribeToEvents()
    {

    }

    protected virtual void UnsubscribeFromEvents()
    {

    }
}
