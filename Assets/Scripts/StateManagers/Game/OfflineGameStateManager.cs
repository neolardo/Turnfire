using System;
using System.Collections;
using UnityEngine;

public class OfflineGameStateManager : MonoBehaviour, IGameStateManager
{
    [SerializeField] private GameplaySettingsDefinition _gameplaySettings;
    private bool _countdownEnded;


    private GameStateType _state;

    public GameStateType CurrentState
    {
        get
        {
            return _state;
        }

        private set
        {
            if (_state != value)
            {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }
    }

    public event Action <GameStateType> StateChanged;

    private void Awake()
    {
        var inputManager = FindFirstObjectByType<LocalGameplayInput>();
        inputManager.TogglePauseGameplayPerformed += OnTogglePauseResumeGameplay;
        inputManager.PrepareForGameStart();
    }

    private void Start()
    {
        GameServices.CountdownTimer.TimerEnded += OnCountdownEnded;
        GameServices.TurnStateManager.GameEnded += OnGameOver;
        StartCoroutine(StartGameAfterCountdown());
    }

    private IEnumerator StartGameAfterCountdown()
    {
        GameServices.CountdownTimer.Restart();
        yield return new WaitUntil(() => _countdownEnded);
        yield return new WaitForSeconds(_gameplaySettings.DelaySecondsAfterCountdown);
       
        //_countdownTimer.gameObject.SetActive(false); //TODO: move
        yield return new WaitUntil(()=> GameServices.TurnStateManager.IsInitialized);
        StartGame();
    }
    private void StartGame()
    {
        CurrentState = GameStateType.Playing;
        GameServices.TurnStateManager.StartGame(SceneLoader.Instance.CurrentGameplaySceneSettings);
    }

    private void OnCountdownEnded()
    {
        _countdownEnded = true;
    }

    private void OnGameOver(Team winnerTeam)
    {
        CurrentState = GameStateType.GameOver;
    }

    private void OnTogglePauseResumeGameplay()
    {
        if(CurrentState == GameStateType.Playing)
        {
            CurrentState = GameStateType.Paused;
        }
        else
        {
            CurrentState = GameStateType.Playing;
        }
    }

}
