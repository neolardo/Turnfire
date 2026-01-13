using System;
using Unity.Netcode;
using UnityEngine;

public class OnlineTimer : NetworkBehaviour, ITimer
{
    [SerializeField] private TimerType _timerType;
    public TimerType TimerType => _timerType;

    private NetworkVariable<double> _endServerTime = new();

    private NetworkVariable<bool> _isRunning =  new();

    private NetworkVariable<bool> _isInitialized = new();

    public float CurrentTime { get; private set; }
    public bool IsRunning => _isRunning.Value;
    public bool IsInitialized => _isInitialized.Value;

    public Func<bool> CanRestart { get; set; }
    public Func<bool> CanPause { get; set; }
    public Func<bool> CanResume { get; set ; }


    public event Action TimerEnded;

    private float _initialTime;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(_timerType == TimerType.Countdown )
        {
            GameServices.RegisterCountdownTimer(this);
        }
        else if (_timerType == TimerType.Gameplay)
        {
            GameServices.RegisterGameplayTimer(this);
        }
        else
        {
            Debug.LogError($"Invalid timer type: {_timerType}");
        }
    }
    public void Initialize(float initialTime)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        _initialTime = initialTime;
        CurrentTime = initialTime;
        _isInitialized.Value = true;
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
        NotifyTimerEndedClientRpc();
    }


    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
    private void NotifyTimerEndedClientRpc()
    {
        TimerEnded?.Invoke();
    }

    public virtual void Restart()
    {
        if (!IsServer || (CanRestart != null && !CanRestart()) )
            return;

        _endServerTime.Value = NetworkManager.Singleton.ServerTime.Time + _initialTime;
        _isRunning.Value = true;
    }

    public virtual void Pause()
    {
        if (!IsServer || (CanPause != null && !CanPause()) )
            return;

        _isRunning.Value = false;
    }

    public virtual void Resume()
    {
        if (!IsServer || (CanResume != null && !CanResume()) )
            return;

        _endServerTime.Value = NetworkManager.Singleton.ServerTime.Time + CurrentTime;
        _isRunning.Value = true;
    }
}
