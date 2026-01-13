using System;
using Unity.Netcode;
using UnityEngine;

public class OnlineGameStateManager : NetworkBehaviour, IGameStateManager
{
    [SerializeField] private GameplaySettingsDefinition _gameplaySettings;
    private GameStateManagerLogic _logic;

    private NetworkVariable<GameStateType> _state = new NetworkVariable<GameStateType>();
    public GameStateType CurrentState => _state.Value;

    public event Action<GameStateType> StateChanged;

    public override void OnNetworkSpawn()
    { 
        base.OnNetworkSpawn();
        _state.OnValueChanged += InvokeStateChanged;
        GameServices.Register(this);
        if (!IsServer)
        { 
            return;
        }
        _logic = new GameStateManagerLogic(_gameplaySettings);
        _logic.StateChanged += ServerOnStateChanged;
    }

    public void StartGame()
    {
        if (!IsServer)
        {
            return;
        }
        StartCoroutine(_logic.StartGameAfterCountdownCoroutine());
    }

    private void InvokeStateChanged(GameStateType oldState, GameStateType newState)
    {
        StateChanged?.Invoke(newState);
    }

    private void ServerOnStateChanged(GameStateType newState)
    {
        if(!IsServer)
        {
            return;
        }
        _state.Value = newState;
    }

    public void TogglePauseResumeGameplay()
    {
        // an online game should not be paused
    }

}
