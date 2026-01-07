using System;
using UnityEngine;

public class OfflineTimer : MonoBehaviour, ITimer
{
    public float CurrentTime { get; private set; }
    public bool IsRunning { get; private set;}
    public bool IsInitialized { get; private set; }
    public Func<bool> CanRestart { get; set; }
    public Func<bool> CanPause { get; set; }
    public Func<bool> CanResume { get; set; }

    protected float _initialTime;

    public event Action TimerEnded;

    public void Initialize(float initialTime)
    {
        _initialTime = initialTime;
        CurrentTime = initialTime;
        IsInitialized = true;
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
        if (CanRestart != null && !CanRestart())
            return;

        CurrentTime = _initialTime;
        IsRunning = true;
    }

    public void Pause()
    {
        if (CanPause != null && !CanPause())
            return;

        IsRunning = false;
    }

    public void Resume()
    {
        if (CanResume != null && !CanResume())
            return;

        IsRunning = true;
    }
}
