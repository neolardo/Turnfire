using System;
using UnityEngine;

public class OfflineTimer : MonoBehaviour, ITimer
{
    public float CurrentTime { get; private set; }
    public bool IsRunning { get; private set;}

    protected float _initialTime;

    public event Action TimerEnded;

    public void Initialize(float initialTime)
    {
        _initialTime = initialTime;
    }

    void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        CurrentTime -= Time.deltaTime;

        if (CurrentTime <= 0)
        {
            OnTimerEnded();
        }
    }

    private void OnTimerEnded()
    {
        CurrentTime = 0;
        IsRunning = false;
        TimerEnded?.Invoke();
    }


    public void Restart()
    {
        CurrentTime = _initialTime;
        IsRunning = true;
    }

    public void Pause()
    {
        IsRunning = false;
    }

    public void Resume()
    {
        IsRunning = true;
    }
}
