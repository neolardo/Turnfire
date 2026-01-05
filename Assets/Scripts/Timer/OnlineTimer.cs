using System;
using Unity.Netcode;
using UnityEngine;

public class OnlineTimer : NetworkBehaviour, ITimer
{
    private NetworkVariable<double> _endServerTime = new(writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> _isRunning =  new(writePerm: NetworkVariableWritePermission.Server);
    public float CurrentTime { get; private set; }
    public bool IsRunning => _isRunning.Value;

    public event Action TimerEnded;

    private float _initialTime;


    public void Initialize(float initialTime)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            return;
        }

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
        NotifyTimerEndedRpc();
    }


    [Rpc(SendTo.Everyone)]
    private void NotifyTimerEndedRpc()
    {
        TimerEnded?.Invoke();
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
