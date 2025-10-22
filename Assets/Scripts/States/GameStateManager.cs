using System;
using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private GameplaySettingsDefinition _gameplaySettings;
    [SerializeField] private CountdownTimerUI _countdownTimer;
    [SerializeField] private TurnManager _turnManager;
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
        var inputManager = FindFirstObjectByType<InputManager>();
        var turnManager = FindFirstObjectByType<TurnManager>();
        inputManager.TogglePauseGameplayPerformed += OnTogglePauseResumeGameplay;
        _countdownTimer.TimerEnded += OnCountdownEnded;
        turnManager.GameEnded += OnGameOver;
    }

    private void Start()
    {
        StartCoroutine(StartGameAfterCountdown());
    }

    private IEnumerator StartGameAfterCountdown()
    {
        _countdownTimer.StartTimer();
        yield return new WaitUntil(() => _countdownEnded);
        yield return new WaitForSeconds(_gameplaySettings.DelaySecondsAfterCountdown);
        _countdownTimer.gameObject.SetActive(false);
        StartGame();
    }
    private void StartGame()
    {
        CurrentState = GameStateType.Playing;
        _turnManager.StartGame(SceneLoader.Instance.CurrentGameplaySceneSettings);
    }

    public void OnCountdownEnded()
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
