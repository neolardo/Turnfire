using System;
using UnityEngine;

public class OfflineTimer : MonoBehaviour, ITimer
{
    public float CurrentTime { get; private set; }
    protected float _initialTime;
    protected bool _isRunning;

    public event Action TimerEnded;

    private void Awake()
    {
        _isRunning = false;
    }

    public void Initialize(float initialTime)
    {
        _initialTime = initialTime;
    }


    void Update()
    {
        if (!_isRunning)
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
        _isRunning = false;
        TimerEnded?.Invoke();
    }


    public void Restart()
    {
        CurrentTime = _initialTime;
        _isRunning = true;
    }

    public void Pause()
    {
        _isRunning = false;
    }

    public void Resume()
    {
        _isRunning = true;
    }
}
