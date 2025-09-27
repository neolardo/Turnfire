using System;
using System.Collections;
using UnityEngine;

public class StateBase
{
    public bool IsActive { get; protected set; }
    protected MonoBehaviour _coroutineManager;

    public event Action StateEnded;

    protected StateBase(MonoBehaviour coroutineManager)
    {
        _coroutineManager = coroutineManager;
    }

    protected void InvokeStateEndedEvent()
    {
        StateEnded?.Invoke();
    }

    protected Coroutine StartCoroutine(IEnumerator routine)
    {
        return _coroutineManager.StartCoroutine(routine);
    }

    protected virtual void SubscribeToEvents()
    {

    }

    protected virtual void UnsubscribeFromEvents()
    {

    }
}
