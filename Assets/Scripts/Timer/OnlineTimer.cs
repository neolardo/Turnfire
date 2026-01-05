using System;
using Unity.Netcode;
using UnityEngine;

public class OnlineTimer : NetworkBehaviour, ITimer
{
    private NetworkVariable<double> _endServerTime = new(writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> _isRunning =  new(writePerm: NetworkVariableWritePermission.Server);

    private float _initialTime;

    public float CurrentTime { get; private set; }

    public event Action TimerEnded;

    public void Initialize(float initialTime)
    {
        _initialTime = initialTime;
    }

    void Update()
    {
        if (!_isRunning.Value)
            return;

        double remaining = _endServerTime.Value - NetworkManager.Singleton.ServerTime.Time;

        CurrentTime = Mathf.Max(0f, (float)remaining);

        if (IsServer && remaining <= 0)
        {
            OnTimerEnded();
        }
    }

    private void OnTimerEnded()
    {
        _isRunning.Value = false;
        CurrentTime = 0f;
        TimerEnded?.Invoke(); // server-side only
    }

    public void Restart()
    {
        if (!IsServer)
            return;

        _endServerTime.Value = NetworkManager.Singleton.ServerTime.Time + _initialTime;

        _isRunning.Value = true;
    }

    public void Pause()
    {
        if (!IsServer)
            return;

        _isRunning.Value = false;
    }

    public void Resume()
    {
        if (!IsServer)
            return;

        _endServerTime.Value = NetworkManager.Singleton.ServerTime.Time + CurrentTime;

        _isRunning.Value = true;
    }
}
