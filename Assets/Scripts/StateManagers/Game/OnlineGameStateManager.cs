using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class OnlineGameStateManager : NetworkBehaviour, IGameStateManager
{
    [SerializeField] private GameplaySettingsDefinition _gameplaySettings;
    private bool _countdownEnded;

    private NetworkVariable<GameStateType> _state = new NetworkVariable<GameStateType>();

    public GameStateType CurrentState
    {
        get
        {
            return _state.Value;
        }

        private set
        {
            if(!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (_state.Value != value)
            {
                _state.Value = value;
                StateChanged?.Invoke(_state.Value);
            }
        }
    }

    public event Action<GameStateType> StateChanged; //TODO: is server only action okay?

    private void Awake()
    {
        var inputManager = FindFirstObjectByType<LocalGameplayInput>();
        inputManager.TogglePauseGameplayPerformed += OnTogglePauseResumeGameplay;
        inputManager.PrepareForGameStart();
    }

    private void Start()
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        GameServices.CountdownTimer.TimerEnded += OnCountdownEnded;
        GameServices.TurnStateManager.GameEnded += OnGameOver;
        StartCoroutine(StartGameAfterCountdown());
    }
    private void OnCountdownEnded()
    {
        _countdownEnded = true;
    }

    private IEnumerator StartGameAfterCountdown()
    {
        GameServices.CountdownTimer.Restart();
        yield return new WaitUntil(() => _countdownEnded);
        yield return new WaitForSeconds(_gameplaySettings.DelaySecondsAfterCountdown);
        yield return new WaitUntil(() => GameServices.TurnStateManager.IsInitialized);
        StartGame();
    }

    private void StartGame()
    {
        CurrentState = GameStateType.Playing;
        GameServices.TurnStateManager.StartGame(SceneLoader.Instance.CurrentGameplaySceneSettings);
    }


    private void OnGameOver(Team winnerTeam)
    {
        if(!IsServer)
        {
            return;
        }

        CurrentState = GameStateType.GameOver;
    }

    private void OnTogglePauseResumeGameplay()
    {
        if (CurrentState == GameStateType.Playing)
        {
            CurrentState = GameStateType.Paused;
        }
        else
        {
            CurrentState = GameStateType.Playing;
        }
    }

}
