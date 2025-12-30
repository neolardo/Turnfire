using System;
using UnityEngine;

public class StateBase : UnityDriven
{
    public bool IsActive { get; protected set; }

    public event Action StateEnded;

    protected StateBase(MonoBehaviour coroutineManager) : base (coroutineManager)
    {
    }

    protected void InvokeStateEndedEvent()
    {
        StateEnded?.Invoke();
    }

    public virtual void OnDestroy()
    {

    }

    protected virtual void SubscribeToEvents()
    {

    }

    protected virtual void UnsubscribeFromEvents()
    {

    }
}
